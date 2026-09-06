using System;
using System.Collections.Generic;
using System.Text;

namespace TlsClient.Core.Models.Entities
{
    // Reference: https://github.com/bogdanfinn/tls-client/blob/master/cffi_src/types.go#L92
    // Reference: https://bogdanfinn.gitbook.io/open-source-oasis/tls-client/custom-client-profile#shared-library-and-standalone-api
    public class CustomTlsClient
    {
        public Dictionary<string, uint> H2Settings { get; set; } = new Dictionary<string, uint>();
        public PriorityParam? HeaderPriority { get; set; }
        public List<string> CertCompressionAlgos { get; set; } = new List<string>();
        public string Ja3String { get; set; } = string.Empty;
        public string TrustAnchorsPayload { get; set; } = string.Empty;
        public List<string> H2SettingsOrder { get; set; } = new List<string>();
        public List<string> KeyShareCurves { get; set; } = new List<string>();
        public List<string> ALPNProtocols { get; set; } = new List<string>();
        public List<string> ALPSProtocols { get; set; } = new List<string>();
        public List<ushort> ECHCandidatePayloads { get; set; } = new List<ushort>();
        public List<ECHCandidateCipherSuite> ECHCandidateCipherSuites { get; set; } = new List<ECHCandidateCipherSuite>();
        public List<PriorityFrame> PriorityFrames { get; set; } = new List<PriorityFrame>();
        public List<string> PseudoHeaderOrder { get; set; } = new List<string>();
        public List<string> SupportedDelegatedCredentialsAlgorithms { get; set; } = new List<string>();
        public List<string> SupportedSignatureAlgorithms { get; set; } = new List<string>();
        public List<string> SupportedVersions { get; set; } = new List<string>();
        public uint ConnectionFlow { get; set; }
        public ushort RecordSizeLimit { get; set; }
    }
}
