using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miwalab.ShadowGroup.Callibration
{
    public enum CallibrationSettingType
    {
        CallibrationImport1,
        CallibrationExport1,
        CallibrationImport2,
        CallibrationExport2,
        CallibrationImport3,
        CallibrationExport3,
        CallibrationImport4,
        CallibrationExport4,
        CameraCllibration1,
        CameraCllibration2,
        RemoteCallibration1,
        RemoteCallibration2,
        Count,
    }

    public enum ProjectionCameraMode
    {
        Orthographic,
        Perthpective
    }
}
