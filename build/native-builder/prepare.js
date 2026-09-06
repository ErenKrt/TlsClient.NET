const path = require('path');
const fs = require('fs');
const fsPromises = fs.promises;

const basePath = process.env.GITHUB_WORKSPACE;

if (!basePath) {
  console.error('GITHUB_WORKSPACE environment variable is not defined.');
  process.exit(1);
}

const tlsVersion = (process.env.TLS_CLIENT_VERSION || '').replace(/^v/, '');

if (!tlsVersion) {
  console.error('TLS_CLIENT_VERSION environment variable is not defined.');
  process.exit(1);
}

const tlsClientLibrariesPath = path.join(
  basePath,
  'build',
  'temp'
);

const nativePath = path.join(
  basePath,
  'src',
  'native'
);

const nativeTemplatePath = path.join(
  nativePath,
  'template'
);

/**
 * Example filenames:
 *
 * tls-client-xgo-1.16.0-windows-amd64.dll
 * tls-client-xgo-1.16.0-windows-386.dll
 * tls-client-xgo-1.16.0-linux-amd64.so
 * tls-client-xgo-1.16.0-linux-arm64.so
 * tls-client-xgo-1.16.0-linux-arm-7.so
 * tls-client-xgo-1.16.0-darwin-amd64.dylib
 * tls-client-xgo-1.16.0-darwin-arm64.dylib
 */

function escapeRegex(value) {
  return value.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

const tlsClientRegex = new RegExp(
  `^tls-client-xgo-${escapeRegex(tlsVersion)}-(windows|linux|darwin)-(.+)\\.(dll|so|dylib)$`
);

console.log('Configuration:');
console.log(`- Base path: ${basePath}`);
console.log(`- TLS Version: ${tlsVersion}`);
console.log(`- Libraries path: ${tlsClientLibrariesPath}`);
console.log(`- Native path: ${nativePath}`);
console.log(`- Template path: ${nativeTemplatePath}`);

/**
 * Replace template variables in string.
 */
function replaceTemplateVars(str, props) {
  return Object.entries(props).reduce(
    (result, [key, value]) => {
      const regex = new RegExp(`{${key}}`, 'g');
      return result.replace(regex, value);
    },
    str
  );
}

/**
 * Copy template directory while replacing variables
 * in filenames and text contents.
 */
async function copyDirWithTemplateReplacement(
  src,
  dest,
  props
) {
  await fsPromises.mkdir(dest, {
    recursive: true,
  });

  const entries = await fsPromises.readdir(src, {
    withFileTypes: true,
  });

  for (const entry of entries) {
    const srcPath = path.join(src, entry.name);

    const destName = replaceTemplateVars(
      entry.name,
      props
    );

    const destPath = path.join(
      dest,
      destName
    );

    if (entry.isDirectory()) {
      await copyDirWithTemplateReplacement(
        srcPath,
        destPath,
        props
      );

      continue;
    }

    const content = await fsPromises.readFile(
      srcPath,
      'utf8'
    );

    const processedContent =
      replaceTemplateVars(
        content,
        props
      );

    await fsPromises.writeFile(
      destPath,
      processedContent
    );
  }
}

/**
 * Convert XGO platform/architecture names
 * into the naming convention used by CycleTLS.NET.
 */
function normalizeLibrary(os, arch) {
  /*
   * Windows
   *
   * XGO:
   *   windows-amd64
   *   windows-386
   *
   * Our NuGet layout:
   *   win/x64
   *   win/x86
   */
  if (os === 'windows') {
    if (arch === 'amd64') {
      return {
        os: 'win',
        arch: 'x64',
        runtimeIdentifier: 'win-x64',
      };
    }

    if (arch === '386') {
      return {
        os: 'win',
        arch: 'x86',
        runtimeIdentifier: 'win-x86',
      };
    }

    return null;
  }

  /*
   * macOS
   *
   * XGO:
   *   darwin-amd64
   *   darwin-arm64
   */
  if (os === 'darwin') {
    if (arch === 'amd64') {
      return {
        os: 'darwin',
        arch: 'amd64',
        runtimeIdentifier: 'osx-x64',
      };
    }

    if (arch === 'arm64') {
      return {
        os: 'darwin',
        arch: 'arm64',
        runtimeIdentifier: 'osx-arm64',
      };
    }

    return null;
  }

  /*
   * Linux
   */
  if (os === 'linux') {
    if (arch === 'amd64') {
      return {
        os: 'linux',
        arch: 'amd64',
        runtimeIdentifier: 'linux-x64',
      };
    }

    if (arch === '386') {
      return {
        os: 'linux',
        arch: '386',
        runtimeIdentifier: 'linux-x86',
      };
    }

    if (arch === 'arm64') {
      return {
        os: 'linux',
        arch: 'arm64',
        runtimeIdentifier: 'linux-arm64',
      };
    }

    /*
     * Keep compatibility with the old
     * tls-client Linux ARMv7 naming.
     *
     * XGO:
     *   linux-arm-7
     *
     * Our layout:
     *   linux/armv7
     */
    if (arch === 'arm-7') {
      return {
        os: 'linux',
        arch: 'armv7',
        runtimeIdentifier: 'linux-arm',
      };
    }

    /*
     * XGO also publishes arm-5 / arm-6,
     * ppc64le, riscv64, s390x etc.
     *
     * We intentionally don't package those
     * until CycleTLS.NET explicitly supports them.
     */
    return null;
  }

  return null;
}

/**
 * Parse a tls-client XGO filename.
 */
function parseLibrary(library) {
  const match = library.match(tlsClientRegex);

  if (!match) {
    return null;
  }

  const [, xgoOs, xgoArch, ext] = match;

  const normalized = normalizeLibrary(
    xgoOs,
    xgoArch
  );

  if (!normalized) {
    console.log(
      `Skipping unsupported target: ${xgoOs}-${xgoArch}`
    );

    return null;
  }

  const {
    os,
    arch,
    runtimeIdentifier,
  } = normalized;

  const fullName =
    `TlsClient.Native.${os}-${arch}`;

  return {
    fullName,

    os,
    arch,
    ext,

    xgoOs,
    xgoArch,

    runtimeIdentifier,

    sourcePath: path.join(
      tlsClientLibrariesPath,
      library
    ),

    originalFileName: library,
  };
}

/**
 * Process all downloaded tls-client libraries.
 */
async function processLibraries() {
  try {
    console.log(
      '\nStarting library processing...'
    );

    const libraries = await fsPromises.readdir(
      tlsClientLibrariesPath
    );

    const processableLibraries = libraries
      .map(parseLibrary)
      .filter(Boolean);

    if (processableLibraries.length === 0) {
      throw new Error(
        `No compatible tls-client XGO libraries found in ${tlsClientLibrariesPath}`
      );
    }

    console.log(
      `\nFound ${processableLibraries.length} compatible libraries:`
    );

    for (const lib of processableLibraries) {
      console.log(
        `- ${lib.originalFileName} -> ${lib.fullName}`
      );
    }

    /*
     * Detect duplicate package IDs before doing
     * any file generation.
     */
    const packageNames = new Set();

    for (const lib of processableLibraries) {
      if (packageNames.has(lib.fullName)) {
        throw new Error(
          `Duplicate native package detected: ${lib.fullName}`
        );
      }

      packageNames.add(lib.fullName);
    }

    for (const lib of processableLibraries) {
      console.log(
        `\nProcessing: ${lib.fullName}`
      );

      console.log(
        `XGO target: ${lib.xgoOs}-${lib.xgoArch}`
      );

      console.log(
        `NuGet runtime: ${lib.runtimeIdentifier}`
      );

      const targetDir = path.join(
        nativePath,
        lib.fullName
      );

      /*
       * Make generation idempotent.
       *
       * Important for local runs or re-runs inside
       * the same workspace.
       */
      await fsPromises.rm(
        targetDir,
        {
          recursive: true,
          force: true,
        }
      );

      const templateProps = {
        title: lib.fullName,

        os: lib.os,
        arch: lib.arch,

        version: tlsVersion,

        ext: lib.ext,

        runtimeIdentifier:
          lib.runtimeIdentifier,
      };

      await copyDirWithTemplateReplacement(
        nativeTemplatePath,
        targetDir,
        templateProps
      );

      /*
       * IMPORTANT:
       *
       * Keep the existing CycleTLS.NET NuGet layout:
       *
       * runtimes/
       *   tls-client/
       *     {platform}/
       *       {arch}/
       *         tls-client.{ext}
       */
      const runtimesPath = path.join(
        targetDir,
        'runtimes',
        'tls-client',
        lib.os,
        lib.arch
      );

      await fsPromises.mkdir(
        runtimesPath,
        {
          recursive: true,
        }
      );

      const destFileName =
        `tls-client.${lib.ext}`;

      const destPath = path.join(
        runtimesPath,
        destFileName
      );

      await fsPromises.copyFile(
        lib.sourcePath,
        destPath
      );

      console.log(
        `✓ ${lib.originalFileName}`
      );

      console.log(
        `  -> runtimes/tls-client/${lib.os}/${lib.arch}/${destFileName}`
      );
    }

    console.log(
      '\nLibrary processing completed successfully.'
    );
  } catch (error) {
    console.error(
      '\nError processing libraries:',
      error
    );

    process.exit(1);
  }
}

processLibraries();