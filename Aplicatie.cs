namespace PRoiect_poo_nou;

public class Aplicatie
{
    private List<Utilizator> utilizatori = new();
    public Hotel Hotel { get; private set; }
    private FisiereSalvate _fisiereSalvate = new();

    public Aplicatie()
    {
        Console.WriteLine("=== Initializare sistem hotel ===");
        
        try
        {
            var (camere, rezervari, utilizatoriIncarcati, config) = _fisiereSalvate.IncarcaDate();
            
            if (camere.Count > 0)
            {
                Console.WriteLine($" Incarcate {camere.Count} camere, {rezervari.Count} rezervari, {utilizatoriIncarcati.Count} utilizatori");
                
                Hotel = new Hotel(camere, rezervari, config);
                utilizatori = utilizatoriIncarcati;//hotel cu date incarcate
                RecreazaAdmini();//adminii cu referința la Hotel
                
                Console.WriteLine(" Date incarcate cu succes!");
            }
            else
            {
                Console.WriteLine(" Prima rulare - creare date inițiale...");
                
                // config default
                Hotel = new Hotel();
                utilizatori = new List<Utilizator>();
                
                // utilizatori default
                creare_utilizator("admin", "admin123", true);
                creare_utilizator("client", "client123", false);
                
                
                SalveazaDate();//aici am salvat date initiale
                
                Console.WriteLine(" Date initiale create si salvate!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Eroare la inițializare: {ex.Message}");
            Console.WriteLine(" Creare hotel cu setari default...");
            
            // Fallback la hotel nou??
            Hotel = new Hotel();
            utilizatori = new List<Utilizator>();
            creare_utilizator("admin", "admin123", true);
            creare_utilizator("client", "client123", false);
            
            // Încearcă să salveze datele default
            try
            {
                SalveazaDate();
            }
            catch
            {
                Console.WriteLine(" Nu s-au putut salva datele default");
            }
        }
    }

    private void RecreazaAdmini()
    {
        //identifica admin(erau slvati ca si client)
        var adminUsers = utilizatori
            .Where(u => u.Nume == "admin" || u is Administrator)
            .ToList();
            
        // Recreăm adminii cu referința corectă la Hotel
        foreach (var admin in adminUsers)
        {
            var newAdmin = new Administrator(admin.Nume, admin.Parola, Hotel, this);
            utilizatori.Remove(admin);
            utilizatori.Add(newAdmin);
        }
    }

    public void SalveazaDate()
    {
        try
        {
            _fisiereSalvate.SalveazaDate(Hotel, utilizatori);
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Eroare la salvarea datelor: {ex.Message}");
        }
    }
    
    public void   creare_utilizator(string nume, string parola, bool isAdmin)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nume))
                throw new ArgumentException("Numele nu poate fi gol");
                
            if (string.IsNullOrWhiteSpace(parola))
                throw new ArgumentException("Parola nu poate fi goala");
                
            // verifică dacă utilizatorul există deja
            if (utilizatori.Any(u => u.Nume.Equals(nume, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Utilizatorul '{nume}' exista deja");
            
            Utilizator utilizator;
            
            if (isAdmin)
            {
                utilizator = new Administrator(nume, parola, Hotel, this);
            }
            else
            {
                utilizator = new Client(nume, parola);
            }
            
            utilizatori.Add(utilizator);
            SalveazaDate();
            Console.WriteLine($" Utilizator '{nume}' creat {(isAdmin ? "(admin)" : "(client)")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Eroare la crearea utilizatorului: {ex.Message}");
            throw;
        }
    }

    public Utilizator? verificare_utilizator(string nume_util, string parola_util)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nume_util) || string.IsNullOrWhiteSpace(parola_util))
            {
                Console.WriteLine("✗ Numele si parola sunt obligatorii");
                return null;
            }
            
            foreach (Utilizator util in utilizatori)
            {
                if (util.Nume.Equals(nume_util, StringComparison.OrdinalIgnoreCase) && 
                    util.Parola == parola_util)
                {
                    return util;
                }
            }
            
            Console.WriteLine("Utilizator sau parolă incorecta");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la verificare: {ex.Message}");
            return null;
        }
    }
}