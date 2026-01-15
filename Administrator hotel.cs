namespace PRoiect_poo_nou;

public class Administrator : Utilizator
{
    public Hotel Hotel { get; set; }//e folosit in mai multe locuri trb acesat usor
    private Aplicatie aplicatie;

    public Administrator(string nume, string parola, Hotel hotel, Aplicatie app)
    {
        if (string.IsNullOrWhiteSpace(nume))
            throw new ArgumentException("Numele este obligatoriu");
        if (string.IsNullOrWhiteSpace(parola))
            throw new ArgumentException("Parola este obligatorie");
        if (hotel == null)
            throw new ArgumentNullException(nameof(hotel));
        if (app == null)
            throw new ArgumentNullException(nameof(app));
            
        Nume = nume;
        Parola = parola;
        Hotel = hotel;
        aplicatie = app;
    }
    
    public void CreareCamera(Camera.Tip_Camera tip, List<Camera.Facilitati> facilitati)
    {
        try
        {
            Hotel.Adaugare_camera(tip, facilitati);
            aplicatie.SalveazaDate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    public void StergereCamera(int NrCamera)
    {
        try
        {
            Hotel.Stergere_camera(NrCamera);
            aplicatie.SalveazaDate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n");
        }
    }

    public void SchimbaStatusCamera(int NrCamera, Camera.Status_camera status)
    {
        try
        {
            Hotel.GetCamera(NrCamera).Schimb_Status(status);
            aplicatie.SalveazaDate();
            Console.WriteLine($"Statusul camerei {NrCamera} schimbat in {status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    public void SchimbaStatusRezervare(int idRezervare, Rezervari.Status_rezervare status)
    {
        try
        {
            foreach (var rez in Hotel.lista_rezarvari)
            {
                if (rez.id == idRezervare)
                {
                    rez.Schimb_Status(status);
                    aplicatie.SalveazaDate();
                    Console.WriteLine($"Statusul rezervarii #{idRezervare} schimbat in {status}");
                    return;
                }
            }
            throw new ArgumentException($"Rezervarea cu ID-ul {idRezervare} nu a fost gasita!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    public void SetareIntervaleOre(TimeOnly ora_start, TimeOnly ora_stop, TimeOnly checkout)
    {
        try
        {
            Hotel.setare_ore(ora_start, ora_stop, checkout);
            aplicatie.SalveazaDate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }
}