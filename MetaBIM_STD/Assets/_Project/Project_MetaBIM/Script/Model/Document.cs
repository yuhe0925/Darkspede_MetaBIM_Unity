
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaBIM
{
    /// <summary>
    /// Record of fi
    /// </summary>

    [Serializable]
    public class Document : IModel
    {
        public string attached = "N";    // attached target (profile, project, bcf, etc...)
        public string createdBy = "";   // guid of profile that created this document
        public string encryptionkey = "n"; // the key to decrypt the resouce 
        public string payload = "n"; // serialized payload of the document resource, optional

        // Must have
        public string documentType          = DocumentType.Text;
        public string documentName          = "nextbimdefault";
        public string documentFileExtension = ".txt";
        public string documentFileUrl       = "";      // url of local source     
        public string documentExternalLink  = "";      // url of external source

        public string documentDescription   = "There is no context";
        public int documentSize = 0;   // in bytes

        public Document()
        {

        }

        public Document(string attached, string createdBy, string documentType, string documentName, string documentFileExtension, string documentFileUrl, string documentDescription, int documentSize)
        {
            this.attached = attached;
            this.createdBy = createdBy;
            this.documentType = documentType;
            this.documentName = documentName;
            this.documentFileExtension = documentFileExtension;
            this.documentFileUrl = documentFileUrl;
            this.documentDescription = documentDescription;
            this.documentSize = documentSize;
        }



        public static string ToJson(Document _item, bool _isMasked = true)
        {
            return JsonConvert.SerializeObject(_item);
        }

        public static Document FromJson(string _json)
        {
            return JsonConvert.DeserializeObject<Document>(_json);
        }

        public static List<Document> FromJsonList(string _json)
        {
            return JsonConvert.DeserializeObject<List<Document>>(_json);
        }

        public new string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }


    }


    public static class DocumentType {
        public static String Image { get { return "image"; } }
        public static String Icon { get { return "icon"; } }
        public static String DOC { get { return "doc"; } }
        public static String Text { get { return "txt"; } }
        public static String AssetBundle { get { return "asset"; } }


        public static String XMD { get { return "xmd"; } }
        public static String PDF { get { return "pdf"; } }
        public static String RVT { get { return "rvt"; } }
        public static String IFC { get { return "ifc"; } }
        public static String FBX { get { return "fbx"; } }
        public static String DAE { get { return "dae"; } }
        public static String MAX { get { return "MAX"; } }
        public static String OBJ { get { return "obj"; } }
        public static String BIN { get { return "bin"; } }
        public static String DXF { get { return "dxf"; } }
        public static String DWG { get { return "DWG"; } }
        public static String XML { get { return "xml"; } }
        public static String CSV { get { return "csv"; } }


        public static string GetPropValue(string _name)
        {
            return DocumentType.GetPropValue(_name.ToUpper());
        }
    }

}


