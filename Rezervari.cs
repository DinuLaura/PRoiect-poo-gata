namespace PRoiect_poo_nou;

public class Rezervari
{
    public int id { get; init; }
    public string Nume_client { get; init; }
    public Camera Camera { get; init; }//referinta nu se schimba
    
    public DateOnly Inceput_rezervare { get; private set; }
    public DateOnly Sfarsit_rezervare { get; private set; }
    public bool activa { get; private set; }
    
    public enum Status_rezervare { REZERVARE_FACUTA, REZERVARE_ANULATA }
    public Status_rezervare Status { get; set; }
    
    public Rezervari(string numeClient, Camera camere, DateOnly inceput_rezervare, 
        DateOnly sfarsit_rezervare, int identificator)
    {
        if (string.IsNullOrWhiteSpace(numeClient))
            throw new ArgumentException("Numele clientului este obligatoriu");
            
        if (camere == null)
            throw new ArgumentNullException(nameof(camere));
            
        if (sfarsit_rezervare < inceput_rezervare)
            throw new ArgumentException("Data de sfarsit trebuie să fie dupa data de inceput");
            
        if (identificator < 0)
            throw new ArgumentException("ID-ul nu poate fi negativ");
            
        Inceput_rezervare = inceput_rezervare;
        Sfarsit_rezervare = sfarsit_rezervare;
        Nume_client = numeClient;
        Camera = camere;
        id = identificator;
        activa = true;
        Status = Status_rezervare.REZERVARE_FACUTA;
    }
    
    public void Schimb_Status(Status_rezervare Status_nou)
    {
        Status = Status_nou;
        activa = (Status_nou == Status_rezervare.REZERVARE_FACUTA);
    }

    public void setare_inceput_rezervare(DateOnly inceput_rezervare)
    {
        if (inceput_rezervare > Sfarsit_rezervare)
            throw new ArgumentException("Data de inceput nu poate fi după data de sfarsit");
            
        Inceput_rezervare = inceput_rezervare;
    }
    
    public void setare_sfarsit_rezervare(DateOnly sfarsit_rezervare)
    {
        if (sfarsit_rezervare < Inceput_rezervare)
            throw new ArgumentException("Data de sfarsit nu poate fi înainte de data de inceput");
            
        Sfarsit_rezervare = sfarsit_rezervare;
    }
    
    public override string ToString()
    {
        return $"Rezervare #{id}: {Nume_client} - Camera {Camera.Nr_camera} ({Inceput_rezervare} → {Sfarsit_rezervare})";
    }
}