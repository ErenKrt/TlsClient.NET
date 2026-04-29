const path = require('path');
const fs = require('fs');
const fsPromises = fs.promises;

// Define paths based on environment variables
const basePath = process.env.GITHUB_WORKSPACE;
const tlsVersion = process.env.TLS_CLIENT_VERSION?.replace('v', '') || '';
// PACKAGE_VERSION is the NuGet package version stamped into the platform-specific
// project file. Defaults to TLS_CLIENT_VERSION when not set so existing CI runs
// (which only set TLS_CLIENT_VERSION) keep working unchanged.
const packageVersion = process.env.PACKAGE_VERSION || tlsVersion;
const tlsClientRegex = new RegExp(`tls-client-(.*)-(.*)-${tlsVersion}\\.(.*)`);

// Set up paths
const tlsClientLibrariesPath = path.join(basePath, 'build', 'temp');
const nativePath = path.join(basePath, 'src', 'native');
const nativeTemplatePath = path.join(nativePath, 'template');

// Log key configuration information
console.log('Configuration:');
console.log(`- Base path: ${basePath}`);
console.log(`- TLS Version: ${tlsVersion}`);
console.log(`- Template path: ${nativeTemplatePath}`);

/**
 * Replace template variables in a string
 * @param {string} str - Input string with template variables
 * @param {object} props - Object containing template variable values
 * @returns {string} String with replaced variables
 */
function replaceTemplateVars(str, props) {
  return Object.entries(props).reduce((result, [key, value]) => {
    const regex = new RegExp(`{${key}}`, 'g');
    return result.replace(regex, value);
  }, str);
}

/**
 * Copy directory with template replacement in both filenames and content
 * @param {string} src - Source directory path
 * @param {string} dest - Destination directory path
 * @param {object} props - Template variables to replace
 */
async function copyDirWithTemplateReplacement(src, dest, props) {
  await fsPromises.mkdir(dest, { recursive: true });
  const entries = await fsPromises.readdir(src, { withFileTypes: true });

  for (const entry of entries) {
    const srcPath = path.join(src, entry.name);
    const destName = replaceTemplateVars(entry.name, props);
    const destPath = path.join(dest, destName);

    if (entry.isDirectory()) {
      await copyDirWithTemplateReplacement(srcPath, destPath, props);
    } else {
      const content = await fsPromises.readFile(srcPath, 'utf8');
      const processedContent = replaceTemplateVars(content, props);
      await fsPromises.writeFile(destPath, processedContent);
    }
  }
}

/**
 * Process libraries and create platform-specific packages
 */
async function processLibraries() {
  try {
    console.log('Starting library processing...');
    const libraries = await fsPromises.readdir(tlsClientLibrariesPath);

    // Process libraries and collect metadata
    const processableLibraries = libraries
      .map(library => {
        const match = library.match(tlsClientRegex);
        if (!match) return null;

        let [, os, arch, ext] = match;
        var runtimeIdentifier;
        // Normalize OS and architecture names
        if (os === 'windows') {
          os = 'win';
          arch = `x${arch}`;
          runtimeIdentifier = `win-${arch === 'x64' ? 'x64' : 'x86'}`;
        }else if (os.includes('linux')) {
          runtimeIdentifier = `linux-${arch === 'amd64' ? 'x64' : (arch === 'arm64' ? 'arm64' : 'arm')}`;
        }else if (os === 'darwin') {
          runtimeIdentifier = `osx-${arch === 'amd64' ? 'x64' : 'arm64'}`;
        }

        const fullName = `TlsClient.Native.${os}-${arch}`;
        const sourcePath = path.join(tlsClientLibrariesPath, library);

        return {
          fullName,
          os,
          arch,
          ext,
          sourcePath,
          runtimeIdentifier,
        };
      })
      .filter(Boolean); // Filter out null values more concisely

    console.log(`Found ${processableLibraries.length} compatible libraries`);

    // Process each library
    for (const lib of processableLibraries) {
      console.log(`\nProcessing: ${lib.fullName}`);

      const targetDir = path.join(nativePath, lib.fullName);

      // Create template replacement props
      const templateProps = {
        title: lib.fullName,
        os: lib.os,
        arch: lib.arch,
        version: tlsVersion,
        packageVersion: packageVersion,
        ext: lib.ext,
        runtimeIdentifier: lib.runtimeIdentifier,
      };

      // Create the project structure with template replacements
      await copyDirWithTemplateReplacement(nativeTemplatePath, targetDir, templateProps);

      // Create runtime directory and copy native library
      const runtimesPath = path.join(targetDir, 'runtimes', 'tls-client', lib.os, lib.arch);
      await fsPromises.mkdir(runtimesPath, { recursive: true });

      const destFileName = `tls-client.${lib.ext}`;
      const destPath = path.join(runtimesPath, destFileName);

      await fsPromises.copyFile(lib.sourcePath, destPath);
      console.log(`✓ Created ${lib.fullName} (${lib.os}-${lib.arch})`);
    }

    console.log('\nLibrary processing completed successfully');
  } catch (error) {
    console.error('Error processing libraries:', error);
    process.exit(1); // Exit with error code for CI/CD pipelines
  }
}

// Run the async function
processLibraries();