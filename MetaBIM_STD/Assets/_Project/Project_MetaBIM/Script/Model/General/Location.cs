using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Location 的摘要说明
/// </summary>
[Serializable]
public class Location : IModel
{
    public string addressLine1 = "4/75 Mark Street";
    public string addressLine2;
    public string city = "North Melbourne";
    public string state = "VIC";
    public int postalCode = 3051;
    public string country = "Australia";
    public string fullAddress = "4/75 Mark Street North Melbourne VIC 3051";


    // the original point
    public double latitude = -37.7945821;
    public double longitude = 144.9373748;
    public double altitude = 0;

    public double rotatino = 0;    // the heading of the site

    public float zoom = 16;                                                                 

    public float offsetX = 0;
    public float offsetY = 0;
    public float offsetZ = 0;


    public Location()
    {

    }
    public Location(float _latitude, float _longitude, float _altitude)
    {
        latitude = _latitude;
        longitude = _longitude;
        altitude = _altitude;

        // get address by lat long
    }

    public Location(string _fullAddress)
    {
        fullAddress = _fullAddress;
    }

    public Location(string addressLine1, string addressLine2, string city, string state, int postalCode, string country)
    {
        this.addressLine1 = addressLine1;
        this.addressLine2 = addressLine2;
        this.city = city;
        this.state = state;
        this.postalCode = postalCode;
        this.country = country;

        fullAddress = GetAddressString();
    }

    public void SetCoordinate(double latitude, double longitude, double altitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = altitude;
    }

    public string GetAddressString()
    {
        string address =
            addressLine1.ToLower().Trim() + " " +
            addressLine2.ToLower().Trim() + " " +
            city.ToLower().Trim() + " " +
            state.ToUpper().Trim() + " " +
            postalCode + " " +
            country.ToUpper().Trim();

        return address;          
    }
}