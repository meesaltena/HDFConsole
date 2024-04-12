public class File
{
    public string Filename { get; set; }
    public int Size { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastModified { get; set; }
}


// curl --location --request GET https://api.dataplatform.knmi.nl/open-data/v1/datasets/radar_forecast/versions/1.0/files/RAD_NL25_PCP_FM_201910261400.h5/url --header "Authorization: eyJvcmciOiI1ZTU1NGUxOTI3NGE5NjAwMDEyYTNlYjEiLCJpZCI6ImE1OGI5NGZmMDY5NDRhZDNhZjFkMDBmNDBmNTQyNjBkIiwiaCI6Im11cm11cjEyOCJ9"