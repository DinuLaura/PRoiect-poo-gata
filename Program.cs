using Microsoft.Extensions.Logging;

namespace PRoiect_poo_nou;

class Program
{
    static Aplicatie app = new Aplicatie();
    static Utilizator? currentUser = null;
    static ILogger logger;

    static void Main(string[] args)
    {
        // Salveaza datele la inchiderea aplicatiei
        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
        {
            Console.WriteLine("\nSalvare date la inchidere...");
            app.SalveazaDate();
            Console.WriteLine("La revedere!");
        };

        // Gestioneaza Ctrl+C
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nSalvare date...");
            app.SalveazaDate();
            Console.WriteLine("La revedere!");
            Environment.Exit(0);
        };

        // Creeaza un logger simplu
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        
        logger = loggerFactory.CreateLogger<Program>();

        Console.WriteLine("=== SISTEM HOTEL SELF CHECK-IN ===");
        Console.WriteLine("Foloseste Ctrl+C pentru a iesi elegant\n");
        
        try
        {
            while (true)
            {
                if (currentUser == null)
                    MeniuPrincipal();
                else if (currentUser is Administrator admin)
                    MeniuAdmin(admin);
                else if (currentUser is Client client)
                    MeniuClient(client);
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Eroare neasteptata: {ex.Message}");
            //Console.WriteLine($"\nEroare neasteptata: {ex.Message}");
            Console.WriteLine("Apasa orice tasta pentru a inchide...");
            Console.ReadKey();
        }
    }

    static void MeniuPrincipal()
    {
        Console.WriteLine("\n--- MENIU PRINCIPAL ---");
        Console.WriteLine("1. Login");
        Console.WriteLine("2. Inregistrare client");
        Console.WriteLine("3. Exit");
        Console.Write("Alege optiunea: ");
        
        string opt = Console.ReadLine()?.Trim() ?? "";
        
        switch (opt)
        {
            case "1": Login(); break;
            case "2": InregistrareClient(); break;
            case "3": 
                Console.WriteLine("La revedere!");
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Optiune invalida!");
                break;
        }
    }

    static void Login()
    {
        try
        {
            Console.Write("\nNume utilizator: ");
            string user = Console.ReadLine()?.Trim() ?? "";
            Console.Write("Parola: ");
            string pass = Console.ReadLine()?.Trim() ?? "";
            
            currentUser = app.verificare_utilizator(user, pass);
            
            if (currentUser != null)
            {
                logger.LogInformation($"Login reusit: {user}");
                Console.WriteLine($"\nBun venit, {user}!");
                if (currentUser is Administrator)
                    Console.WriteLine("Modul: Administrator");
                else
                    Console.WriteLine("Modul: Client");
            }
            else
            {
                logger.LogWarning($"Login esuat pentru: {user}");
                Console.WriteLine("Nume utilizator sau parola incorecta!");
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Eroare login: {ex.Message}");
            Console.WriteLine($"Eroare la login: {ex.Message}");
        }
    }

    static void InregistrareClient()
    {
        try
        {
            Console.WriteLine("\n--- INREGISTRARE CLIENT ---");
            Console.Write("Nume utilizator: ");
            string user = Console.ReadLine()?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(user))
            {
                Console.WriteLine("Numele nu poate fi gol!");
                return;
            }
            
            Console.Write("Parola: ");
            string pass = Console.ReadLine()?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(pass))
            {
                Console.WriteLine("Parola nu poate fi goala!");
                return;
            }
            
            logger.LogInformation($"Inregistrare noua: {user}");
            app.creare_utilizator(user, pass, false);
            Console.WriteLine($"\nCont creat cu succes! Autentifica-te acum.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Eroare inregistrare: {ex.Message}");
            Console.WriteLine($"{ex.Message}");
        }
    }

    static void MeniuAdmin(Administrator admin)
    {
        Console.WriteLine("\n--- MENIU ADMINISTRATOR ---");
        Console.WriteLine("1. Adauga camera");
        Console.WriteLine("2. Sterge camera");
        Console.WriteLine("3. Vezi camere");
        Console.WriteLine("4. Schimba status camera");
        Console.WriteLine("5. Seteaza intervale ore");
        Console.WriteLine("6. Vezi rezervari active");
        Console.WriteLine("7. Vezi toate rezervarile");
        Console.WriteLine("8. Gestioneaza utilizatori");
        Console.WriteLine("9. Logout");
        Console.Write("Alege optiunea: ");
        
        string opt = Console.ReadLine()?.Trim() ?? "";
        
        switch (opt)
        {
            case "1": AdaugaCamera(admin); break;
            case "2": StergeCamera(admin); break;
            case "3": VeziCamere(admin); break;
            case "4": SchimbaStatusCamera(admin); break;
            case "5": SeteazaIntervaleOre(admin); break;
            case "6": admin.Hotel.Vizualizare_Rezervari_Active(); break;
            case "7": admin.Hotel.Vizualizare_Rezervari(); break;
            case "8": GestioneazaUtilizatori(); break;
            case "9": 
                currentUser = null; 
                Console.WriteLine("Delogat cu succes!");
                break;
            default:
                Console.WriteLine("Optiune invalida!");
                break;
        }
    }

    static void AdaugaCamera(Administrator admin)
    {
        try
        {
            Console.WriteLine("\n--- ADAUGARE CAMERA ---");
            Console.WriteLine("Tip camera:");
            Console.WriteLine("1. SINGLE");
            Console.WriteLine("2. DOUBLE");
            Console.WriteLine("3. GRUP");
            Console.Write("Alege tipul: ");
            
            Camera.Tip_Camera tip;
            switch (Console.ReadLine()?.Trim())
            {
                case "1": tip = Camera.Tip_Camera.CAMERA_SINGLE; break;
                case "2": tip = Camera.Tip_Camera.CAMERA_DOUBLE; break;
                case "3": tip = Camera.Tip_Camera.CAMERA_GRUP; break;
                default:
                    Console.WriteLine("Tip invalid!");
                    return;
            }
            
            // --- COD NOU (DE LIPIT) ---
            List<Camera.Facilitati> facs = new();
            bool continuaSelectia = true;

            while (continuaSelectia)
            {
                Console.Clear(); // Optional: Curata consola pentru claritate
                Console.WriteLine("\n--- SELECTIE FACILITATI ---");
                Console.WriteLine($"Facilitati selectate curent: {(facs.Any() ? string.Join(", ", facs) : "Niciuna")}");
                Console.WriteLine("---------------------------");
                Console.WriteLine("1. Adauga JACUZZI");
                Console.WriteLine("2. Adauga BOARDGAMES");
                Console.WriteLine("3. Adauga BUCATARIE");
                Console.WriteLine("4. Adauga MIFFY");
                Console.WriteLine("0. TERMINA SELECTIA (Gata)");
                Console.Write("Alege o optiune: ");

                string optiune = Console.ReadLine()?.Trim() ?? "";

                switch (optiune)
                {
                    case "1":
                        if (!facs.Contains(Camera.Facilitati.JACUZZI)) 
                            facs.Add(Camera.Facilitati.JACUZZI);
                        else 
                            Console.WriteLine(" -> Deja selectat!");
                        break;
                    case "2":
                        if (!facs.Contains(Camera.Facilitati.BOAREDGAMES)) 
                            facs.Add(Camera.Facilitati.BOAREDGAMES);
                        else 
                            Console.WriteLine(" -> Deja selectat!");
                        break;
                    case "3":
                        if (!facs.Contains(Camera.Facilitati.BUCATARIE)) 
                            facs.Add(Camera.Facilitati.BUCATARIE);
                        else 
                            Console.WriteLine(" -> Deja selectat!");
                        break;
                    case "4":
                        if (!facs.Contains(Camera.Facilitati.MIFFY)) 
                            facs.Add(Camera.Facilitati.MIFFY);
                        else 
                            Console.WriteLine(" -> Deja selectat!");
                        break;
                    case "0":
                        continuaSelectia = false;
                        break;
                    default:
                        Console.WriteLine("Optiune invalida! Apasa Enter.");
                        Console.ReadKey();
                        break;
                }
            }
            
            admin.CreareCamera(tip, facs);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    static void StergeCamera(Administrator admin)
    {
        try
        {
            Console.Write("\nNumar camera de sters: ");
            if (int.TryParse(Console.ReadLine(), out int nr))
            {
                admin.StergereCamera(nr);
            }
            else
            {
                Console.WriteLine("Numar invalid!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    // in Program.cs
    static void VeziCamere(Administrator admin)
    {
        Console.WriteLine("\n--- LISTA CAMERE ---");
        if (!admin.Hotel.lista_camere.Any())
        {
            Console.WriteLine("Nu exista camere.");
            return;
        }
        
        foreach (var c in admin.Hotel.lista_camere)
        {
            string statusIcon = c.Status switch
            {
                Camera.Status_camera.LIBERA => "[Libera]",
                Camera.Status_camera.OCUPAT => "[Ocupata]",
                Camera.Status_camera.IN_CURATENIE => "[Curatenie]",
                Camera.Status_camera.INDISPONIBILA => "[Indisponibila]",
                _ => "[Necunoscut]"
            };

            // --- LINIILE ADAUGATE/MODIFICATE ---
            // Verificam daca exista facilitati si le unim cu virgula
            string facilitatiText = c.Facilitati_cam.Any() 
                ? string.Join(", ", c.Facilitati_cam) 
                : "Fara facilitati";
            
            // Afisam informatia completa
            Console.WriteLine($"{statusIcon} Camera {c.Nr_camera}: {c.Tip} | Facilitati: {facilitatiText}");
            // -----------------------------------
        }
    }

    static void SchimbaStatusCamera(Administrator admin)
    {
        try
        {
            Console.Write("\nNumar camera: ");
            if (!int.TryParse(Console.ReadLine(), out int nr))
            {
                Console.WriteLine("Numar invalid!");
                return;
            }
            
            Console.WriteLine("\nStatus nou:");
            Console.WriteLine("1. LIBERA");
            Console.WriteLine("2. OCUPATA");
            Console.WriteLine("3. IN CURATENIE");
            Console.WriteLine("4. INDISPONIBILA");
            Console.Write("Alege statusul: ");
            
            Camera.Status_camera status;
            switch (Console.ReadLine()?.Trim())
            {
                case "1": status = Camera.Status_camera.LIBERA; break;
                case "2": status = Camera.Status_camera.OCUPAT; break;
                case "3": status = Camera.Status_camera.IN_CURATENIE; break;
                case "4": status = Camera.Status_camera.INDISPONIBILA; break;
                default:
                    Console.WriteLine("Status invalid!");
                    return;
            }
            
            admin.SchimbaStatusCamera(nr, status);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    static void SeteazaIntervaleOre(Administrator admin)
    {
        try
        {
            Console.WriteLine("\n--- SETARE INTERVALE ORE ---");
            
            Console.Write("Ora start check-in (HH:mm): ");
            if (!TimeOnly.TryParse(Console.ReadLine(), out TimeOnly oraStart))
            {
                Console.WriteLine("Ora invalida!");
                return;
            }
            
            Console.Write("Ora stop check-in (HH:mm): ");
            if (!TimeOnly.TryParse(Console.ReadLine(), out TimeOnly oraStop))
            {
                Console.WriteLine("Ora invalida!");
                return;
            }
            
            Console.Write("Ora checkout (HH:mm): ");
            if (!TimeOnly.TryParse(Console.ReadLine(), out TimeOnly oraCheckout))
            {
                Console.WriteLine("Ora invalida!");
                return;
            }
            
            admin.SetareIntervaleOre(oraStart, oraStop, oraCheckout);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    static void GestioneazaUtilizatori()
    {
        Console.WriteLine("\n--- GESTIUNE UTILIZATORI ---");
        Console.WriteLine("1. Creaza utilizator nou");
        Console.WriteLine("2. Inapoi");
        Console.Write("Alege optiunea: ");
        
        switch (Console.ReadLine()?.Trim())
        {
            case "1": CreazaUtilizator(); break;
            case "2": break;
            default: Console.WriteLine("Optiune invalida!"); break;
        }
    }

    static void CreazaUtilizator()
    {
        try
        {
            Console.Write("\nNume utilizator: ");
            string user = Console.ReadLine()?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(user))
            {
                Console.WriteLine("Numele nu poate fi gol!");
                return;
            }
            
            Console.Write("Parola: ");
            string pass = Console.ReadLine()?.Trim() ?? "";
            
            if (string.IsNullOrWhiteSpace(pass))
            {
                Console.WriteLine("Parola nu poate fi goala!");
                return;
            }
            
            Console.Write("Este administrator? (da/nu): ");
            bool isAdmin = Console.ReadLine()?.Trim().ToLower() == "da";
            
            logger.LogInformation($"Admin creaza utilizator: {user}, admin={isAdmin}");
            app.creare_utilizator(user, pass, isAdmin);
            Console.WriteLine($"Utilizator creat!");
        }
        catch (Exception ex)
        {
            logger.LogError($"Eroare creare utilizator: {ex.Message}");
            Console.WriteLine($"{ex.Message}");
        }
    }

    static void MeniuClient(Client client)
    {
        Console.WriteLine("\n--- MENIU CLIENT ---");
        Console.WriteLine("1. Cauta camere");
        Console.WriteLine("2. Rezerva camera");
        Console.WriteLine("3. Vezi rezervarile mele");
        Console.WriteLine("4. Anuleaza rezervare");
        Console.WriteLine("5. Logout");
        Console.Write("Alege optiunea: ");
        
        string opt = Console.ReadLine()?.Trim() ?? "";
        
        switch (opt)
        {
            case "1": CautaCamere(); break;
            case "2": RezervaCamera(client); break;
            case "3": app.Hotel.afisare_rezervari(client.Nume); break;
            case "4": AnuleazaRezervare(); break;
            case "5": 
                currentUser = null; 
                Console.WriteLine("Delogat cu succes!");
                break;
            default:
                Console.WriteLine("Optiune invalida!");
                break;
        }
    }

    static void CautaCamere()
    {
        try
        {
            Console.WriteLine("\n--- CAUTARE CAMERE ---");
            
            Console.Write("Data inceput (dd/mm/aaaa): ");
            if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly start))
            {
                Console.WriteLine("Data invalida!");
                return;
            }
            
            Console.Write("Data sfarsit (dd/mm/aaaa): ");
            if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly end))
            {
                Console.WriteLine("Data invalida!");
                return;
            }
            
            if (end < start)
            {
                Console.WriteLine("Data de sfarsit trebuie sa fie dupa data de inceput!");
                return;
            }
            
            Console.WriteLine("\nTip camera:");
            Console.WriteLine("1. SINGLE");
            Console.WriteLine("2. DOUBLE");
            Console.WriteLine("3. GRUP");
            Console.Write("Alege tipul: ");
            
            Camera.Tip_Camera tip;
            switch (Console.ReadLine()?.Trim())
            {
                case "1": tip = Camera.Tip_Camera.CAMERA_SINGLE; break;
                case "2": tip = Camera.Tip_Camera.CAMERA_DOUBLE; break;
                case "3": tip = Camera.Tip_Camera.CAMERA_GRUP; break;
                default:
                    Console.WriteLine("Tip invalid!");
                    return;
            }
            
            var camere = app.Hotel.cauta_camere(start, end, tip, new List<Camera.Facilitati>());
            
            Console.WriteLine($"\nGasite {camere.Count} camere disponibile:");
            if (camere.Any())
            {
                foreach (var c in camere)
                {
                    Console.WriteLine($" Camera {c.Nr_camera} - {c.Tip} - Facilitati: {string.Join(", ", c.Facilitati_cam)}");
                }
            }
            else
            {
                Console.WriteLine("Nu exista camere disponibile pentru criteriile selectate.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    static void RezervaCamera(Client client)
    {
        try
        {
            Console.WriteLine("\n--- REZERVARE CAMERA ---");
            
            Console.Write("Numar camera: ");
            if (!int.TryParse(Console.ReadLine(), out int nr))
            {
                Console.WriteLine("Numar invalid!");
                return;
            }
            
            Console.Write("Data inceput (dd/mm/aaaa): ");
            if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly start))
            {
                Console.WriteLine("Data invalida!");
                return;
            }
            
            Console.Write("Data sfarsit (dd/mm/aaaa): ");
            if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly end))
            {
                Console.WriteLine("Data invalida!");
                return;
            }
            
            if (end < start)
            {
                Console.WriteLine("Data de sfarsit trebuie sa fie dupa data de inceput!");
                return;
            }
            
            app.Hotel.rezervare_camera(client.Nume, start, end, nr);
            app.SalveazaDate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }

    static void AnuleazaRezervare()
    {
        try
        {
            Console.Write("\nID rezervare de anulat: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                app.Hotel.anulare_rezervare(id);
                app.SalveazaDate();
            }
            else
            {
                Console.WriteLine("ID invalid!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare: {ex.Message}");
        }
    }
}