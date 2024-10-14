using Unity.Services.Core.Editor;
using UnityEditor.CrashReporting;
using UnityEngine;

namespace Unity.Services.CloudDiagnostics.Editor
{
    class CloudDiagnosticsEditorGameServiceEnabler : EditorGameServiceFlagEnabler
    {
        protected override string FlagName { get; } = "gameperf";

        protected override void EnableLocalSettings()
        {
            CrashReportingSettings.enabled = true;
        }

        protected override void DisableLocalSettings()
        {
            CrashReportingSettings.enabled = false;
        }

        public override bool IsEnabled()
        {
            return CrashReportingSettings.enabled;
        }
    }
}
