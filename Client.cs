namespace PRoiect_poo_nou;

public class Client : Utilizator
{
    public Client(string nume, string parola)
    {
        Nume = nume ?? throw new ArgumentNullException(nameof(nume));//arunca eroare daca e gol daca nu o pastreaza
        Parola = parola ?? throw new ArgumentNullException(nameof(parola));
    }
}