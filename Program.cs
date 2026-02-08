using System;
using System.Collections.Generic;
using System.Linq;

namespace PlanZajecProjekt
{
    // ----- TERMIN -----
    class Termin
    {
        public DayOfWeek Dzien { get; set; }
        public int Godzina { get; set; }

        public bool CzyTenSam(Termin inny)
            => Dzien == inny.Dzien && Godzina == inny.Godzina;

        public override string ToString()
            => $"{Dzien}, godz. {Godzina}";
    }

    // ----- SALA -----
    class Sala
    {
        public string Numer { get; set; }   // np. RA117a, Aula
        public int Pojemnosc { get; set; }

        public override string ToString()
            => $"Sala {Numer} (poj. {Pojemnosc})";
    }

    // ----- KLASA BAZOWA -----
    abstract class Zajecia
    {
        public string Kierunek { get; set; }
        public string Przedmiot { get; set; }
        public string Prowadzacy { get; set; }
        public Sala Sala { get; set; }
        public Termin Termin { get; set; }

        public abstract bool CzyDlaGrupy(int nrGrupy);

        public virtual void Wyswietl()
            => Console.WriteLine($"{Termin} | {Sala} | {Przedmiot} | {Prowadzacy}");
    }

    // ----- LABORATORIUM -----
    class Laboratorium : Zajecia
    {
        public int NrGrupy { get; set; }

        public override bool CzyDlaGrupy(int nrGrupy)
            => NrGrupy == nrGrupy;

        public override void Wyswietl()
        {
            base.Wyswietl();
            Console.WriteLine($"   Typ: Laboratorium | Grupa: {NrGrupy}");
        }
    }

    // ----- WYKŁAD -----
    class Wyklad : Zajecia
    {
        public override bool CzyDlaGrupy(int nrGrupy) => true;

        public override void Wyswietl()
        {
            base.Wyswietl();
            Console.WriteLine("   Typ: Wykład");
        }
    }

    // ----- PROJEKT -----
    class Projekt : Zajecia
    {
        public int Grupa1 { get; set; }
        public int Grupa2 { get; set; }

        public override bool CzyDlaGrupy(int nrGrupy)
            => nrGrupy == Grupa1 || nrGrupy == Grupa2;

        public override void Wyswietl()
        {
            base.Wyswietl();
            Console.WriteLine($"   Typ: Projekt | Grupy: {Grupa1}, {Grupa2}");
        }
    }

    // ----- PLAN ZAJĘĆ -----
    class PlanZajec
    {
        private List<Zajecia> lista = new List<Zajecia>();

        public void Dodaj(Zajecia z)
        {
            foreach (var istnieje in lista)
            {
                if (istnieje.Termin.CzyTenSam(z.Termin))
                {
                    if (istnieje.Sala.Numer.Equals(z.Sala.Numer, StringComparison.OrdinalIgnoreCase))
                        throw new Exception("❌ Sala jest już zajęta!");

                    for (int g = 1; g <= 30; g++)
                        if (istnieje.CzyDlaGrupy(g) && z.CzyDlaGrupy(g))
                            throw new Exception("❌ Grupa ma już zajęcia w tym czasie!");
                }
            }
            lista.Add(z);
        }

        public void Usun(int index)
        {
            if (index < 0 || index >= lista.Count)
                throw new Exception("Nieprawidłowy numer!");

            lista.RemoveAt(index);
        }

        public void WyswietlWszystkie()
        {
            if (lista.Count == 0)
            {
                Console.WriteLine("Brak zajęć.");
                return;
            }

            for (int i = 0; i < lista.Count; i++)
            {
                Console.WriteLine($"[{i}]");
                lista[i].Wyswietl();
            }
        }

        public void WyswietlDzien(DayOfWeek dzien)
            => lista.Where(z => z.Termin.Dzien == dzien)
                    .ToList()
                    .ForEach(z => z.Wyswietl());

        public void WyswietlGrupe(int grupa)
            => lista.Where(z => z.CzyDlaGrupy(grupa))
                    .ToList()
                    .ForEach(z => z.Wyswietl());

        public void WyswietlSale(string sala)
            => lista.Where(z => z.Sala.Numer.Equals(sala, StringComparison.OrdinalIgnoreCase))
                    .ToList()
                    .ForEach(z => z.Wyswietl());
    }

    // ----- PROGRAM -----
    class Program
    {
        static void Main()
        {
            PlanZajec plan = new PlanZajec();
            DodajDaneStartowe(plan);

            while (true)
            {
                Console.WriteLine("\n--- SYSTEM PLANU ZAJĘĆ ---");
                Console.WriteLine("1. Dodaj laboratorium");
                Console.WriteLine("2. Dodaj wykład");
                Console.WriteLine("3. Dodaj projekt");
                Console.WriteLine("4. Pokaż plan dla dnia");
                Console.WriteLine("5. Pokaż plan dla grupy");
                Console.WriteLine("6. Pokaż plan dla sali");
                Console.WriteLine("7. Pokaż wszystkie zajęcia");
                Console.WriteLine("8. Usuń zajęcia");
                Console.WriteLine("0. Wyjście");
                Console.Write("Wybór: ");

                if (!int.TryParse(Console.ReadLine(), out int wybor))
                {
                    Console.WriteLine("Niepoprawny wybór.");
                    continue;
                }

                try
                {
                    if (wybor == 0) break;

                    switch (wybor)
                    {
                        case 1: plan.Dodaj(TworzLaboratorium()); break;
                        case 2: plan.Dodaj(TworzWyklad()); break;
                        case 3: plan.Dodaj(TworzProjekt()); break;
                        case 4: plan.WyswietlDzien(WybierzDzien()); break;
                        case 5:
                            Console.Write("Nr grupy: ");
                            plan.WyswietlGrupe(int.Parse(Console.ReadLine()));
                            break;
                        case 6:
                            Console.Write("Sala (np. RA117a): ");
                            plan.WyswietlSale(Console.ReadLine());
                            break;
                        case 7: plan.WyswietlWszystkie(); break;
                        case 8:
                            plan.WyswietlWszystkie();
                            Console.Write("Numer zajęć do usunięcia: ");
                            plan.Usun(int.Parse(Console.ReadLine()));
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        // ----- DANE STARTOWE -----
        static void DodajDaneStartowe(PlanZajec plan)
        {
            // ----- PONIEDZIAŁEK -----
            plan.Dodaj(new Wyklad
            {
                Kierunek = "6IIZ",
                Przedmiot = "Programowanie Obiektowe",
                Prowadzacy = "Dr inż. Fryc",
                Sala = new Sala { Numer = "RA117a", Pojemnosc = 25 },
                Termin = new Termin { Dzien = DayOfWeek.Monday, Godzina = 7 }
            });

            // ----- WTOREK -----
            plan.Dodaj(new Laboratorium
            {
                Kierunek = "6IIZ",
                Przedmiot = "Programowanie Obiektowe",
                Prowadzacy = "Dr inż. Fryc",
                Sala = new Sala { Numer = "RA250a", Pojemnosc = 35 },
                Termin = new Termin { Dzien = DayOfWeek.Tuesday, Godzina = 3 },
                NrGrupy = 1
            });

            // ----- ŚRODA -----
            plan.Dodaj(new Projekt
            {
                Kierunek = "6IIZ",
                Przedmiot = "Projekt Bazy Danych",
                Prowadzacy = "Dr Wiśniewski",
                Sala = new Sala { Numer = "RA250b", Pojemnosc = 35 },
                Termin = new Termin { Dzien = DayOfWeek.Wednesday, Godzina = 4 },
                Grupa1 = 1,
                Grupa2 = 2
            });

            // ----- CZWARTEK -----
            plan.Dodaj(new Wyklad
            {
                Kierunek = "6IIZ",
                Przedmiot = "Algorytmy i Struktury Danych",
                Prowadzacy = "dr Nowak",
                Sala = new Sala { Numer = "Aula", Pojemnosc = 120 },
                Termin = new Termin { Dzien = DayOfWeek.Thursday, Godzina = 1 }
            });

            plan.Dodaj(new Laboratorium
            {
                Kierunek = "6IIZ",
                Przedmiot = "Algorytmy i Struktury Danych",
                Prowadzacy = "mgr Kaczmarek",
                Sala = new Sala { Numer = "KA101", Pojemnosc = 28 },
                Termin = new Termin { Dzien = DayOfWeek.Thursday, Godzina = 3 },
                NrGrupy = 2
            });

            // ----- PIĄTEK -----
            plan.Dodaj(new Wyklad
            {
                Kierunek = "6IIZ",
                Przedmiot = "Inżynieria Oprogramowania",
                Prowadzacy = "dr Mazur",
                Sala = new Sala { Numer = "RA220", Pojemnosc = 70 },
                Termin = new Termin { Dzien = DayOfWeek.Friday, Godzina = 2 }
            });

            plan.Dodaj(new Projekt
            {
                Kierunek = "6IIZ",
                Przedmiot = "Projekt Inżynierski",
                Prowadzacy = "dr Zieliński",
                Sala = new Sala { Numer = "RA117b", Pojemnosc = 25 },
                Termin = new Termin { Dzien = DayOfWeek.Friday, Godzina = 4 },
                Grupa1 = 2,
                Grupa2 = 3
            });
        }


        // ----- METODY POMOCNICZE -----
        static DayOfWeek WybierzDzien()
        {
            Console.WriteLine("1-Monday  2-Tuesday  3-Wednesday  4-Thursday  5-Friday");
            int d;
            while (!int.TryParse(Console.ReadLine(), out d) || d < 1 || d > 5) { }
            return (DayOfWeek)d;
        }

        static Sala TworzSale()
        {
            Console.Write("Sala: ");
            string numer = Console.ReadLine();

            Console.Write("Pojemność: ");
            int poj;
            while (!int.TryParse(Console.ReadLine(), out poj) || poj <= 0) { }

            return new Sala { Numer = numer, Pojemnosc = poj };
        }

        static Termin TworzTermin()
        {
            DayOfWeek dzien = WybierzDzien();

            Console.Write("Godzina (1–10): ");
            int g;
            while (!int.TryParse(Console.ReadLine(), out g) || g < 1 || g > 10) { }

            return new Termin { Dzien = dzien, Godzina = g };
        }

        static Laboratorium TworzLaboratorium()
        {
            Console.Write("Kierunek: ");
            string k = Console.ReadLine();
            Console.Write("Przedmiot: ");
            string p = Console.ReadLine();
            Console.Write("Prowadzący: ");
            string pr = Console.ReadLine();
            Console.Write("Nr grupy: ");
            int g = int.Parse(Console.ReadLine());

            return new Laboratorium
            {
                Kierunek = k,
                Przedmiot = p,
                Prowadzacy = pr,
                Sala = TworzSale(),
                Termin = TworzTermin(),
                NrGrupy = g
            };
        }

        static Wyklad TworzWyklad()
        {
            Console.Write("Kierunek: ");
            string k = Console.ReadLine();
            Console.Write("Przedmiot: ");
            string p = Console.ReadLine();
            Console.Write("Prowadzący: ");
            string pr = Console.ReadLine();

            return new Wyklad
            {
                Kierunek = k,
                Przedmiot = p,
                Prowadzacy = pr,
                Sala = TworzSale(),
                Termin = TworzTermin()
            };
        }

        static Projekt TworzProjekt()
        {
            Console.Write("Kierunek: ");
            string k = Console.ReadLine();
            Console.Write("Przedmiot: ");
            string p = Console.ReadLine();
            Console.Write("Prowadzący: ");
            string pr = Console.ReadLine();
            Console.Write("Grupa 1: ");
            int g1 = int.Parse(Console.ReadLine());
            Console.Write("Grupa 2: ");
            int g2 = int.Parse(Console.ReadLine());

            return new Projekt
            {
                Kierunek = k,
                Przedmiot = p,
                Prowadzacy = pr,
                Sala = TworzSale(),
                Termin = TworzTermin(),
                Grupa1 = g1,
                Grupa2 = g2
            };
        }
    }
}
