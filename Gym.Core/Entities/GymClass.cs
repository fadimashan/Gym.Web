using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Entities
{
    public class GymClass
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime EndDate => StartDate + Duration;

        public string Discription { get; set; }

        public ICollection<ApplicationUserGymClass> AttedingMembers { get; set; }



    }
}
