namespace PRoiect_poo_nou;
//mai usor acc la cke_in cek_out
public class HotelConfig
{
    public TimeOnly ora_start_checkin { get; init; }
    public TimeOnly ora_stop_checkin { get; init; }
    public TimeOnly ora_checkout { get; init; }

    public HotelConfig()
    {
        ora_start_checkin = new TimeOnly(12, 0);
        ora_stop_checkin = new TimeOnly(20, 0);
        ora_checkout = new TimeOnly(12, 0);
    }

    public HotelConfig(TimeOnly start, TimeOnly stop, TimeOnly checkout)
    {
        ora_start_checkin = start;
        ora_stop_checkin = stop;
        ora_checkout = checkout;
    }
}