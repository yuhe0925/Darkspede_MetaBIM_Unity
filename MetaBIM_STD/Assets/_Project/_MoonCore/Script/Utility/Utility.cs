#define DEBUG // Remove this for production

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using System.Data;



/// <summary>
/// Summary description for Utility
/// </summary>
public class Utility
{
    public static Regex EMAIL_PATTERN = new Regex(@"/^ ([\w -] + (?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w -]{0,66})\.([a - z]{2,6}(?:\.[a-z]{2})?)$/i");
    public static Regex PHONE_PATTERN_AUS_1 = new Regex(@"/\+614[0-9]{8}$/");
    public static Regex PHONE_PATTERN_AUS_2 = new Regex(@"/04[0 - 9]{8}$/");
    public static Regex PHONE_PATTERN_AUS_3 = new Regex(@"/4[0 - 9]{8}$/");
    public static Regex PHONE_PATTERN_AUS_4 = new Regex(@"/614[0-9]{8}$/");
    public static string COUNTRY_AUS = "aus";



    /* ============================================================================
     * =================== User IDs Validation     ================================
     * ============================================================================
     */


    /// <summary>
    /// Checking the format and validating the mobile input
    /// </summary>
    /// <param name="_mobile"></param>
    /// <param name="_country">The short extension of country, example "aus" = Australia</param>
    /// <returns>Return "" if the mobile is not valid, otherwise return the normalized number</returns>
    public static string ValidateMobile(string _mobile, string _country = "aus")
    {
        string number = _mobile;

        if (_country == COUNTRY_AUS)
        {
            _mobile = _mobile.Replace(" ", "");
            _mobile = _mobile.Replace("-", "");
            _mobile = _mobile.Replace("+", "");


            if (_mobile.StartsWith("614") && _mobile.Length == 11)
            {
                if (_mobile.All(char.IsDigit))
                {
                    return _mobile;
                }
                return "";
            }

            if (_mobile.StartsWith("04") && _mobile.Length == 10)
            {
                _mobile = _mobile.Remove(0, 1);
                _mobile = "61" + _mobile;
                if (_mobile.All(char.IsDigit))
                {
                    return _mobile;
                }
                return "";
            }

            if (_mobile.StartsWith("4") && _mobile.Length == 9)
            {
                _mobile = "61" + _mobile;
                if (_mobile.All(char.IsDigit))
                {
                    return _mobile;
                }
                return "";
            }


        }

        //other countries
        return null;
    }

    public static string GetHashedMobileNUmber(string _mobile, long _hash)
    {
        long mobile = long.Parse(_mobile);
        long hashedNumber = mobile * _hash;
        return hashedNumber.ToString();
    }

    public static string GetMobileNumberFromHash(string _hashed, long _hash)
    {
        long hashedNumber = long.Parse(_hashed);
        long mobile = hashedNumber / _hash;
        return mobile.ToString();
    }

    public static string GetSubedMobileNumber(string _mobile)
    {
 
        if(_mobile == null || _mobile.Length < 8)
        {
            return "";
        }

        StringBuilder subbedMobile = new StringBuilder(_mobile);
        subbedMobile[5] = '*';
        subbedMobile[6] = '*';
        subbedMobile[7] = '*';
        subbedMobile[8] = '*';

        return subbedMobile.ToString();
    } 

    public static bool ValidateEmail(string _email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(_email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /* ============================================================================
     * =================== Date Time Process CH/ZN ================================
     * ============================================================================
     */
    #region  Date Time Process CH/ZN

    public static List<string> CurrentDate()
    {
        string year = System.DateTime.Now.Year.ToString();
        string month = System.DateTime.Now.Month.ToString();
        string day = System.DateTime.Now.Day.ToString();
        string dayofWeek = System.DateTime.Now.DayOfWeek.ToString();
        string temp = "null";
        var currentDate = new List<string>();
        currentDate.Add(month);
        currentDate.Add(day);
        currentDate.Add(year);


        switch (dayofWeek)
        {
            case "Monday":
                temp = "一";
                break;
            case "Tuesday":
                temp = "二";
                break;
            case "Wednesday":
                temp = "三";
                break;
            case "Thursday":
                temp = "四";
                break;
            case "Friday":
                temp = "五";
                break;
            case "Saturday":
                temp = "六";
                break;
            case "Sunday":
                temp = "日";
                break;
            default:
                break;

        }
        currentDate.Add(temp);

        return currentDate;

    }
    public static DateTime DataStringToData(string _dateString)
    {
        DateTime result = Convert.ToDateTime(_dateString);
        return result;
    }

    /// <summary>
    /// get the day of the week in ZH chinese
    /// 获取中文星期几
    /// </summary>
    /// <param name="_date"></param>
    /// <returns></returns>
    public static string GetDayofWeekZH(DateTime _date)
    {
        string temp = "null";

        switch (_date.DayOfWeek.ToString())
        {
            case "Monday":
                temp = "星期一";
                break;
            case "Tuesday":
                temp = "星期二";
                break;
            case "Wednesday":
                temp = "星期三";
                break;
            case "Thursday":
                temp = "星期四";
                break;
            case "Friday":
                temp = "星期五";
                break;
            case "Saturday":
                temp = "星期六";
                break;
            case "Sunday":
                temp = "星期天";
                break;
            default:
                break;

        }

        return temp;
    }

    public static string GetDayDiffFromNow(string _updated)
    {
        DateTime.Parse(_updated);
        int day = (int)(DateTime.Now - DateTime.Parse(_updated)).TotalDays;
        if (day == 0)
        {
            return "[今天]";
        }
        else
        {
            return "[" + day + "天前]";
        }


    }

    public static int GetHourDiffFromNow(DateTime _to)
    {
        return (DateTime.Now - _to).Hours;
    }

    public static int GetMinuteDiffFromNow(DateTime _to)
    {
        return (DateTime.Now - _to).Minutes;
    }

    public static int GetTimeDiffInMinutes(DateTime _Start, DateTime _End)
    {
        DateTime startTime = _Start;
        DateTime endTime = _End;
        TimeSpan span = endTime.Subtract(startTime);
        return (int)span.TotalMinutes;
    }

    public static double DateTimeDiffMinutes(DateTime _a, DateTime _b)
    {
        return (_b - _a).TotalMinutes;
    }

    public static double DateTimeDiffSeconds(DateTime _a, DateTime _b)
    {
        return (_b - _a).TotalSeconds;
    }

    #endregion



    public static bool SecurityCheck()
    {
        return false;
    }

    public static String GetSHA(String _value)
    {
        StringBuilder Sb = new StringBuilder();

        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(_value));

            foreach (Byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        Debug.Log(Sb.ToString());
        return Sb.ToString();
    }

    public static string GetTempCelsius(float _kelvin)
    {
        float result = 0;
        result = _kelvin - 273.15f;
        return result.ToString("0.0");
    }

    public static string GetNewGUID()
    {
        return Guid.NewGuid().ToString();
    }

    public static string GetStringFromStringList(List<string> _list)
    {
        string s = "[";

        int i = 0;
        foreach(string line in _list)
        {
            if(i == 0)
            {
                s = s + line;
            }
            else
            {
                s = s + "," + line;
            }
            i++;
        }

        s = s + "]";
        return s;
    }

    public static List<string> GetStringListFromString(string _string)
    {
        List<string> StringList = new List<string>();
        string newString = _string.Remove(0,1);
        newString = newString.Remove(newString.Length-1, 1);
        string[] list = newString.Split(',');
        foreach (string line in list)
        {
            StringList.Add(line);
        }
        return StringList;
    }

    public static string GetExcelColumnName(int columnNumber)
    {
        int dividend = columnNumber;
        string columnName = String.Empty;
        int modulo;

        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
            dividend = (int)((dividend - modulo) / 26);
        }

        return columnName;
    }

    public static string UppercaseFirst(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }


    //:::    unit = the unit you desire for results                               :::
    //:::           where: 'M' is statute miles (default)                         :::
    //:::                  'K' is kilometers                                      :::
    //:::                  'N' is nautical miles                                  :::
    //:::                                                                         :::
    public static double GetDistance(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
    {
        if ((lat1 == lat2) && (lon1 == lon2))
        {
            return 0;
        }
        else
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }
    }

    //This function converts decimal degrees to radians 
    private static double deg2rad(double deg)
    {
        return (deg * Math.PI / 180.0);
    }

    //This function converts radians to decimal degrees
    private static double rad2deg(double rad)
    {
        return (rad / Math.PI * 180.0);
    }

    public static string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    public static string GetLastPartOfGuid(string guid)
    {
        string[] list = guid.Split('-');
        return list[list.Length - 1];
    }




    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Image Process                                                 :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    #region Image Process

  
    public static Texture2D ResampleAndCrop(Texture2D source, int targetWidth, int targetHeight)
    {
        int sourceWidth = source.width;
        int sourceHeight = source.height;
        float sourceAspect = (float)sourceWidth / sourceHeight;
        float targetAspect = (float)targetWidth / targetHeight;
        int xOffset = 0;
        int yOffset = 0;
        float factor = 1;
        if (sourceAspect > targetAspect)
        { // crop width
            factor = (float)targetHeight / sourceHeight;
            xOffset = (int)((sourceWidth - sourceHeight * targetAspect) * 0.5f);
        }
        else
        { // crop height
            factor = (float)targetWidth / sourceWidth;
            yOffset = (int)((sourceHeight - sourceWidth / targetAspect) * 0.5f);
        }
        Color32[] data = source.GetPixels32();
        Color32[] data2 = new Color32[targetWidth * targetHeight];
        for (int y = 0; y < targetHeight; y++)
        {
            for (int x = 0; x < targetWidth; x++)
            {
                var p = new Vector2(Mathf.Clamp(xOffset + x / factor, 0, sourceWidth - 1), Mathf.Clamp(yOffset + y / factor, 0, sourceHeight - 1));
                // bilinear filtering
                var c11 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                var c12 = data[Mathf.FloorToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                var c21 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.FloorToInt(p.y))];
                var c22 = data[Mathf.CeilToInt(p.x) + sourceWidth * (Mathf.CeilToInt(p.y))];
                var f = new Vector2(Mathf.Repeat(p.x, 1f), Mathf.Repeat(p.y, 1f));
                data2[x + y * targetWidth] = Color.Lerp(Color.Lerp(c11, c12, p.y), Color.Lerp(c21, c22, p.y), p.x);
            }
        }

        var tex = new Texture2D(targetWidth, targetHeight);
        tex.SetPixels32(data2);
        tex.Apply(true);
        return tex;
    }



    #endregion




    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Unity Related Methods                                         :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    #region Unity Related
    public static string LoadSystemConfig(string _key, string _default)
    {
        string value = _default;

        if (!PlayerPrefs.HasKey(_key))
        {
            PlayerPrefs.SetString(_key, _default);
        }

        value = PlayerPrefs.GetString(_key);
        return value;
    }

    public static void SaveSystemConfig(string _key, string _value)
    {
        PlayerPrefs.SetString(_key, _value);
    }

    public static Vector2 SizeToParent(RawImage image, float padding = 0)
    {
        float w = 0, h = 0;
        var parent = image.GetComponentInParent<RectTransform>();
        var imageTransform = image.GetComponent<RectTransform>();

        // check if there is something to do
        if (image.texture != null)
        {
            if (!parent) { return imageTransform.sizeDelta; } //if we don't have a parent, just return our current width;
            padding = 1 - padding;
            float ratio = image.texture.width / (float)image.texture.height;
            var bounds = new Rect(0, 0, parent.rect.width, parent.rect.height);
            if (Mathf.RoundToInt(imageTransform.eulerAngles.z) % 180 == 90)
            {
                //Invert the bounds if the image is rotated
                bounds.size = new Vector2(bounds.height, bounds.width);
            }
            //Size by height first
            h = bounds.height * padding;
            w = h * ratio;
            if (w > bounds.width * padding)
            { //If it doesn't fit, fallback to width;
                w = bounds.width * padding;
                h = w / ratio;
            }
        }
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        imageTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
        return imageTransform.sizeDelta;
    }

    public static void SetupBlendMode(Material material, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case BlendMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case BlendMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case BlendMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }


    public static Vector3 GetNormalofTriangle(Vector3 _p1, Vector3 _p2, Vector3 _p3)
    {
        Vector3 v1 = _p2 - _p1;
        Vector3 v2 = _p3 - _p1;
        Vector3 normal = Vector3.Cross(v1, v2);
        return normal.normalized;
    }


    public static Direction GetDirectionChecked(Vector3 _objectNormal, Vector3 _checkagaist)
    {
        return Direction.up;
    }


    public enum Direction
    {
        up,
        down,
        left,
        right,
        toward,
        back,
        
    }


    public static bool IsPointWithinCollider(Collider collider, Vector3 point)
    {
        return (collider.ClosestPoint(point) - point).sqrMagnitude < Mathf.Epsilon * Mathf.Epsilon;
    }

    public static bool GetOverlapArea(Bounds a, Bounds b, out OverlapArea overlapArea)
    {
        overlapArea = default;

        // If they are not intersecting we can stop right away ;)
        if (!a.Intersects(b)) return false;

        overlapArea = new OverlapArea(a, b);

        return true;
    }



    public static float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 o)
    {
        Vector3 v1 = p1 - o;
        Vector3 v2 = p2 - o;
        Vector3 v3 = p3 - o;

        return Vector3.Dot(Vector3.Cross(v1, v2), v3) / 6f; ;
    }

    public static float VolumeOfMesh(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        Vector3 o = new Vector3(0f, 0f, 0f);
        // Computing the center mass of the polyhedron as the fourth element of each mesh
        for (int i = 0; i < triangles.Length; i++)
        {
            o += vertices[triangles[i]];
        }
        o = o / mesh.triangles.Length;

        // Computing the sum of the volumes of all the sub-polyhedrons
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];
            volume += SignedVolumeOfTriangle(p1, p2, p3, o);
        }

        return Mathf.Abs(volume);
    }



    #endregion




    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    //::  Time and zone                                                 :::
    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

    public static int GetClientTimeZone(DateTime _dataTime)
    {
        // get time zone int for current client system
        TimeZoneInfo localZone = TimeZoneInfo.Local;
        return localZone.GetUtcOffset(_dataTime).Hours;
    }


    public static string TimeFromTick(string _tick, string _region = "au")
    {
        DateTime time = new DateTime(long.Parse(_tick));
        DateTime utc = time.AddHours(GetClientTimeZone(time));
         
        if(_region == "au")
        {
            return time.ToString("dd-MM-yyyy HH:mm");
        }
        else
        {
            return time.ToString("yyyy-MM-dd HH:mm");
        }

    }


    public static string FormatFileSize(int _size)
    {
        string sizeString = "";


        if(_size < 1024)
        {
            sizeString = _size + "B";
        }
        else if (_size < 1024 * 1024)
        {
            sizeString = _size / 1024 + "KB";
        }
        else
        {
            sizeString = _size / 1024 / 1024 + "MB";
        }

        return sizeString;

    }


    public static string[] GetWordsInHyperWord(string _input)
    {
        string pattern = @"(?<!^)(?=[A-Z])";
        string[] result = Regex.Split(_input, pattern);
        return result;
    }

    
}

public class PropertyCopier<TParent, TChild> where TParent : class
                                            where TChild : class
{
    public static void Copy(TParent parent, TChild child)
    {
        var parentProperties = parent.GetType().GetProperties();
        var childProperties = child.GetType().GetProperties();

        foreach (var parentProperty in parentProperties)
        {
            foreach (var childProperty in childProperties)
            {
                if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                {
                    childProperty.SetValue(child, parentProperty.GetValue(parent));
                    break;
                }
            }
        }
    }


 
}


/// <summary>
/// Source:
/// https://stackoverflow.com/questions/69516211/how-can-i-find-the-volume-and-vertices-of-the-intersection-between-two-axis-alig
/// </summary>
public class OverlapArea
{
    public readonly Vector3 min;
    public readonly Vector3 max;

    public readonly float volume;
    public Vector3 frontBottomLeft => min;
    public readonly Vector3 frontBottomRight;
    public readonly Vector3 frontTopLeft;
    public readonly Vector3 frontTopRight;
    public readonly Vector3 backBottomLeft;
    public readonly Vector3 backBottomRight;
    public readonly Vector3 backTopLeft;
    public Vector3 backTopRight => max;

    public readonly Bounds bounds;

    public OverlapArea(Bounds a, Bounds b)
    {
        // The min and max points
        var minA = a.min;
        var maxA = a.max;
        var minB = b.min;
        var maxB = b.max;

        min.x = Mathf.Max(minA.x, minB.x);
        min.y = Mathf.Max(minA.y, minB.y);
        min.z = Mathf.Max(minA.z, minB.z);

        max.x = Mathf.Min(maxA.x, maxB.x);
        max.y = Mathf.Min(maxA.y, maxB.y);
        max.z = Mathf.Min(maxA.z, maxB.z);

        frontBottomRight = new Vector3(max.x, min.y, min.z);
        frontTopLeft = new Vector3(min.x, max.y, min.z);
        frontTopRight = new Vector3(max.x, max.y, min.z);
        backBottomLeft = new Vector3(min.x, min.y, max.z);
        backBottomRight = new Vector3(max.x, min.y, max.z);
        backTopLeft = new Vector3(min.x, max.y, max.z);

        // The diagonal of this overlap box itself
        var diagonal = max - min;
        volume = diagonal.x * diagonal.y * diagonal.z;

        bounds.SetMinMax(min, max);
    }

    public static bool GetOverlapArea(Bounds a, Bounds b, out OverlapArea overlapArea)
    {
        overlapArea = default;

        // If they are not intersecting we can stop right away ;)
        if (!a.Intersects(b)) return false;

        overlapArea = new OverlapArea(a, b);

        return true;
    }
}