using System;
using XYZ;

namespace MyApp
{
    internal class Programs
    {
        static void Main(string[] args)
        {
            List<Human> humans = new List<Human>();

            Human human1 = new Human();
            human1.Name = "Dmitry";

            Human human2 = new Human();
            human2.Name = "Pasha";

            Human human3 = new Human();
            human3.Name = "Denis";

            humans.Add(human1);
            human2.Add(human2);
            human3.Add(human3);

            foreach (Human human in humans)
            {
                human.ShowName();
            }
        }
    }


}
