namespace PRoiect_poo_nou;

public class Camera
{
    public int Nr_camera { get; private set; }
    public enum Status_camera { LIBERA, OCUPAT, IN_CURATENIE, INDISPONIBILA }
    public enum Tip_Camera { CAMERA_SINGLE, CAMERA_DOUBLE, CAMERA_GRUP }
    public enum Facilitati { JACUZZI, BOAREDGAMES, BUCATARIE, MIFFY }
    
    public Status_camera Status { get; set; }
    public Tip_Camera Tip { get; init; }
    public List<Facilitati> Facilitati_cam { get; init; }
    public TimeOnly Checkin { get; private set; }//daca erau init nu mai mergeau schimbate
    public TimeOnly Checkout { get; private set; }
    
    public Camera(int nrCamera, Tip_Camera tip, List<Facilitati> facilitati_cam)
    {
        if (nrCamera < 0)
            throw new ArgumentException("Numarul camerei nu poate fi negativ");
            
        Nr_camera = nrCamera;
        Tip = tip;
        Facilitati_cam = facilitati_cam ?? new List<Facilitati>();
        Status = Status_camera.LIBERA;
    }

    public void Schimb_Status(Status_camera Status_nou)
    {
        Status = Status_nou;
    }

    public void marcheaza_checkin(TimeOnly checkin)
    {
        Checkin = checkin;
    }
    
    public void marcheaza_checkout(TimeOnly checkout)
    {
        Checkout = checkout;
    }
    
    public override string ToString()
    {
        return $"Camera {Nr_camera} ({Tip}) - {Status}";
    }
}