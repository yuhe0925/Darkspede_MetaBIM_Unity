using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using netDxf;
using netDxf.Header;
using Linefy;

public class CadManager
{


    public static DxfDocument LoadedDocument;

    public static string CadLoader(string _filePath, out DxfDocument _document)
    {

        // this check is optional but recommended before loading a DXF file
        DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(_filePath);

        // netDxf is only compatible with AutoCad2000 and higher DXF versions
        Debug.Log("DxfDocument Version = " + dxfVersion.ToString());
        
        if (dxfVersion < DxfVersion.AutoCad2000)
        {

            _document = null;
            return "version";
        }

        // load file
        LoadedDocument = DxfDocument.Load(_filePath);

        // TODO, make a copy of this loaded document
        _document = LoadedDocument;

        return "done";
    }








    /* Cad Entity */
    #region Cad Entity

    #endregion




    /* Graphic */
    #region Graphic Basic

    public List<GameObject> CAD_Render_Lines;
    public List<GameObject> CAD_Render_Text;
    public void Draw_Line()
    {
        
    }
    #endregion
}




