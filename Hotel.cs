namespace PRoiect_poo_nou;

public class Hotel
{
    public TimeOnly ora_start_checkin { get; private set; }
    public TimeOnly ora_stop_checkin { get; private set; }
    public TimeOnly ora_checkout { get; private set; }
    public List<Camera> lista_camere { get; set; }
    private int cnt = 0;//pt cam cnt stie ce id sa puna la camera ca sa aiba id uri separate
    private int cnt_rezervari = 0;
    public List<Rezervari> lista_rezarvari { get; set; }

    public Hotel(List<Camera>? camere = null, List<Rezervari>? rezervari = null, HotelConfig? config = null)//face parametru null daca nu exista nimic in el
    {
        lista_camere = camere ?? new List<Camera>(); //lista primeste val camerii daca e daca nu face lista noua
        lista_rezarvari = rezervari ?? new List<Rezervari>();
        
        if (lista_camere.Any())
            cnt = lista_camere.Max(c => c.Nr_camera) + 1;//returneaza cel mai mare id pe care are o camera si adauga 1
        
        if (lista_rezarvari.Any())
            cnt_rezervari = lista_rezarvari.Max(r => r.id) + 1;
        
        if (config != null)//ca sa nu mai conf iar
        {
            ora_start_checkin = config.ora_start_checkin;
            ora_stop_checkin = config.ora_stop_checkin;
            ora_checkout = config.ora_checkout;
        }
        else
        {
            ora_start_checkin = new TimeOnly(12, 0);
            ora_stop_checkin = new TimeOnly(20, 0);
            ora_checkout = new TimeOnly(12, 0);
        }
    }
    public void Adaugare_camera(Camera.Tip_Camera tip, List<Camera.Facilitati> facilitati)
    {
        try
        {
            Camera camera_noua = new Camera(cnt++, tip, facilitati ?? new List<Camera.Facilitati>());
            lista_camere.Add(camera_noua);
            Console.WriteLine($"Camera {camera_noua.Nr_camera} adaugată cu succes!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la adăugarea camerei: {ex.Message}");
            throw;
        }
    }

    public void Stergere_camera(int Nrcameraid)
    {
        try
        {
            var camera = lista_camere.FirstOrDefault(c => c.Nr_camera == Nrcameraid);//cauta prima camera care are numaru vrut ca argument
            if (camera != null)
            {
                bool areRezervariActive = lista_rezarvari.Any(r => r.Camera.Nr_camera == Nrcameraid && r.activa);
                
                if (areRezervariActive)//nu o sterge daca are rez act
                {
                    throw new InvalidOperationException(
                        $"Camera {Nrcameraid} are rezervari active. Anulati rezervarile inainte de stergere.");
                }
                
                lista_camere.Remove(camera);//daca n are
                lista_rezarvari.RemoveAll(r => r.Camera.Nr_camera == Nrcameraid);
            }
            else
            {
                throw new ArgumentException($"Camera cu numarul {Nrcameraid} nu exista!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
            throw;
        }
    }

    public Camera GetCamera(int NrCameraid)
    {
        foreach (var cam in lista_camere)
        {
            if (cam.Nr_camera == NrCameraid)
                return cam;
        }
        throw new KeyNotFoundException($"Camera cu numarul {NrCameraid} nu a fost gasita!");
    }
    
    public void Adaugare_rezervari(Rezervari rezervare)
    {
        if (rezervare == null)
            throw new ArgumentNullException(nameof(rezervare));
            
        lista_rezarvari.Add(rezervare);
    }

    public void Vizualizare_Rezervari()
    {
        if (!lista_rezarvari.Any())
        {
            Console.WriteLine("Nu exista rezervari.");
            return;
        }
        
        Vizualizare_Rezervari_List(lista_rezarvari);
    }

    public void Vizualizare_Rezervari_Active()
    {
        List<Rezervari> rezervari_active = new List<Rezervari>();
        foreach (var rezervare in lista_rezarvari)
        {
            if (rezervare.activa)
                rezervari_active.Add(rezervare);
        }
        
        if (!rezervari_active.Any())
        {
            Console.WriteLine("Nu exista rezervari active.");
            return;
        }
        
        Vizualizare_Rezervari_List(rezervari_active);
    }

    private void Vizualizare_Rezervari_List(List<Rezervari> rezervari)
    {
        Console.WriteLine($"\n--- REZERVARI ({rezervari.Count}) ---");
        int counter = 1;
        foreach (var rezervare in rezervari)
        {
            Console.WriteLine($"{counter++}. {rezervare}");
        }
    }

    public void setare_ore(TimeOnly in_cek, TimeOnly out_cek, TimeOnly cekout)
    {
        if (out_cek <= in_cek)
            throw new ArgumentException("Ora de stop check-in trebuie să fie dupa ora de start");
            
        ora_start_checkin = in_cek;
        ora_stop_checkin = out_cek;
        ora_checkout = cekout;
        Console.WriteLine($"Ore setate: Check-in {in_cek}-{out_cek}, Check-out {cekout}");
    }

    public List<Camera> cauta_camere(DateOnly start_perioada, DateOnly sfarsit_perioada, Camera.Tip_Camera tip, List<Camera.Facilitati> facilitati)//dupa periada,fac
    {
        if (sfarsit_perioada < start_perioada)
            throw new ArgumentException("Data de sfarsit trebuie să fie după data de inceput");
            
        List<Camera> camere_gasite = new List<Camera>();
        
        foreach(Camera camera in lista_camere)
        {
            // tip
            if (!camera.Tip.Equals(tip))
                continue;

            // fac
            bool facilitati_indeplinite = true;
            if (facilitati != null)
            {
                foreach (Camera.Facilitati facilitate in facilitati)
                {
                    if (!camera.Facilitati_cam.Contains(facilitate))
                    {
                        facilitati_indeplinite = false;
                        break;
                    }
                }
            }

            if (!facilitati_indeplinite)
                continue;

            //disp
            if (intercalare_rezervari(camera, start_perioada, sfarsit_perioada))
                continue;
            
            // status
            if (camera.Status != Camera.Status_camera.LIBERA)
                continue;
            
            camere_gasite.Add(camera);
        }
        
        return camere_gasite;
    }

    private bool intercalare_rezervari(Camera camera, DateOnly start_perioada, DateOnly sfarsit_perioada)
    {
        foreach (Rezervari rezervare in lista_rezarvari)
        {
            if (rezervare.Camera.Nr_camera == camera.Nr_camera && rezervare.activa)
            {
                if (rezervare.Inceput_rezervare <= sfarsit_perioada && 
                    rezervare.Sfarsit_rezervare >= start_perioada)
                    return true;
            }
        }
        return false;
    }

    private Camera camera_dupa_id(int id)
    {
        foreach (Camera camera in lista_camere)
        {
            if (camera.Nr_camera == id)
                return camera;
        }
        return null;
    }
    
    public void rezervare_camera(string nume, DateOnly start_rez, DateOnly stop_rez, int nr_cam)
    {
        try
        {
            Camera cam_rez = camera_dupa_id(nr_cam);
            if (cam_rez == null)
            {
                throw new ArgumentException($"Camera {nr_cam} nu exista!");
            }
            
            if (stop_rez < start_rez)
            {
                throw new ArgumentException("Data de sfarsit trebuie să fie dupa data de inceput!");
            }
            
            if (intercalare_rezervari(cam_rez, start_rez, stop_rez))
            {
                throw new InvalidOperationException(
                    $"Camera {nr_cam} este indisponibila în perioada {start_rez} - {stop_rez}!");
            }

            Rezervari rezervare = new Rezervari(nume, cam_rez, start_rez, stop_rez, cnt_rezervari++);
            lista_rezarvari.Add(rezervare);
            Console.WriteLine($"Rezervare #{rezervare.id} creata cu succes!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
            throw;
        }
    }
    
    public void afisare_rezervari(string nume)
    {
        if (string.IsNullOrWhiteSpace(nume))
            throw new ArgumentException("Numele este obligatoriu");
            
        Console.WriteLine($"\n--- REZERVARI PENTRU {nume} ---");
        bool gasite = false;
        
        foreach (Rezervari rez in lista_rezarvari)
        {
            if (rez.Nume_client == nume)
            {
                Console.WriteLine($"ID: {rez.id}, Camera: {rez.Camera.Nr_camera}, " +
                    $"Perioada: {rez.Inceput_rezervare} → {rez.Sfarsit_rezervare}, " +
                    $"Status: {rez.Status} {(rez.activa ? "(activa)" : "(anulata)")}");
                gasite = true;
            }
        }
        
        if (!gasite)
            Console.WriteLine("Nu aveti rezervari.");
    }
    
    public void anulare_rezervare(int id)
    {
        var rezervare = lista_rezarvari.FirstOrDefault(r => r.id == id);
        if (rezervare != null)
        {
            if (!rezervare.activa)
            {
                Console.WriteLine("Aceasta rezervare este deja anulata.");
                return;
            }
            
            rezervare.Schimb_Status(Rezervari.Status_rezervare.REZERVARE_ANULATA);
            Console.WriteLine($"Rezervarea #{id} a fost anulata!");
        }
        else
        {
            throw new ArgumentException($"Rezervarea cu ID-ul {id} nu exista!");
        }
    }
    
    public void modificare_rezervare(Rezervari rezervare, DateOnly new_start_date, DateOnly new_end_date)
    {
        if (rezervare == null)
            throw new ArgumentNullException(nameof(rezervare));
            
        if (new_end_date < new_start_date)
            throw new ArgumentException("Data de sfarsit trebuie să fie dupa data de inceput");
            
        if (!rezervare.activa)
            throw new InvalidOperationException("Nu se poate modifica o rezervare anulata");
            
        if (intercalare_rezervari(rezervare.Camera, new_start_date, new_end_date))
        {
            throw new InvalidOperationException(
                "Camera este indisponibila pentru noua perioada!");
        }
        
        rezervare.setare_inceput_rezervare(new_start_date);
        rezervare.setare_sfarsit_rezervare(new_end_date);
        Console.WriteLine($"Rezervarea #{rezervare.id} a fost modificata!");
    }
}