using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArisHotel
{
    public class Session
    {
        static public ArisHotelContext context = new ArisHotelContext();

        static public User currentUser;
    }
}
