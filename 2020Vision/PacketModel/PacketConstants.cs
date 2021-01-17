using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision2020
{
    public class StockInfo
    {
        public int id { get; set; }
        public String name { get; set; }

        public String _shortName { get; set; }
        public string shortName
        {
            get
            {
                if (_shortName == null)
                {
                    _shortName = name.Substring(0, 3).ToUpper();
                }
                return _shortName;
            }
        }
    }

    public static class Constants
    {
        public enum StockType { stTeam, stCountry, stDriver, stTrack, stSurface, stPenalty, stInfringment, stSessionType}
        public static List<StockInfo> TeamList = new List<StockInfo>()
        {
            new StockInfo() {id=0, name="Mercedes"},
            new StockInfo() {id=1, name="Ferrari"},
            new StockInfo() {id=2, name="Red Bull Racing"},
            new StockInfo() {id=3, name="Williams"},
            new StockInfo() {id=4, name="Racing Point"},
            new StockInfo() {id=5, name="Renault"},
            new StockInfo() {id=6, name="Alpha Tauri"},
            new StockInfo() {id=7, name="Haas"},
            new StockInfo() {id=8, name="McLaren"},
            new StockInfo() {id=9, name="Alfa Romeo"},
            new StockInfo() {id=10, name="McLaren 1988"},
            new StockInfo() {id=11, name="McLaren 1991"},
            new StockInfo() {id=12, name="Williams 1992"},
            new StockInfo() {id=13, name="Ferrari 1995"},
            new StockInfo() {id=14, name="Williams 1996"},
            new StockInfo() {id=15, name="McLaren 1998"},
            new StockInfo() {id=16, name="Ferrari 2002"},
            new StockInfo() {id=17, name="Ferrari 2004"},
            new StockInfo() {id=18, name="Renault 2006"},
            new StockInfo() {id=19, name="Ferrari 2007"},
            new StockInfo() {id=20, name="McLaren 2008"},
            new StockInfo() {id=21, name="Red Bull 2010"},
            new StockInfo() {id=22, name="Ferrari 1976"},
            new StockInfo() {id=23, name="ART Grand Prix"},
            new StockInfo() {id=24, name="Campos Vexatec Racing"},
            new StockInfo() {id=25, name="Carlin"},
            new StockInfo() {id=26, name="Charouz Racing System"},
            new StockInfo() {id=27, name="DAMS"},
            new StockInfo() {id=28, name="Russian Time"},
            new StockInfo() {id=29, name="MP Motorsport"},
            new StockInfo() {id=30, name="Pertamina"},
            new StockInfo() {id=31, name="McLaren 1990"},
            new StockInfo() {id=32, name="Trident"},
            new StockInfo() {id=33, name="BWT Arden"},
            new StockInfo() {id=34, name="McLaren 1976"},
            new StockInfo() {id=35, name="Lotus 1972"},
            new StockInfo() {id=36, name="Ferrari 1979"},
            new StockInfo() {id=37, name="McLaren 1982"},
            new StockInfo() {id=38, name="Williams 2003"},
            new StockInfo() {id=39, name="Brawn 2009"},
            new StockInfo() {id=40, name="Lotus 1978"},
            new StockInfo() {id=41, name="F1 Generic car"},
            new StockInfo() {id=42, name="Art GP ’19"},
            new StockInfo() {id=43, name="Campos ’19"},
            new StockInfo() {id=44, name="Carlin ’19"},
            new StockInfo() {id=45, name="Sauber Junior Charouz ’19"},
            new StockInfo() {id=46, name="Dams ’19"},
            new StockInfo() {id=47, name="Uni-Virtuosi ‘19"},
            new StockInfo() {id=48, name="MP Motorsport ‘19"},
            new StockInfo() {id=49, name="Prema ’19"},
            new StockInfo() {id=50, name="Trident ’19"},
            new StockInfo() {id=51, name="Arden ’19"},
            new StockInfo() {id=53, name="Benetton 1994"},
            new StockInfo() {id=54, name="Benetton 1995"},
            new StockInfo() {id=55, name="Ferrari 2000"},
            new StockInfo() {id=56, name="Jordan 1991"},
            new StockInfo() {id= 255, name="My Team "}
        };
        public static List<StockInfo> CountryList = new List<StockInfo>()
        {
            new StockInfo() {id=1, name="American", _shortName="USA"},
            new StockInfo() {id=2, name="Argentinean"},
            new StockInfo() {id=3, name="Australian"},
            new StockInfo() {id=4, name="Austrian", _shortName="AUT" },
            new StockInfo() {id=5, name="Azerbaijani"},
            new StockInfo() {id=6, name="Bahraini"},
            new StockInfo() {id=7, name="Belgian"},
            new StockInfo() {id=8, name="Bolivian"},
            new StockInfo() {id=9, name="Brazilian"},
            new StockInfo() {id=10, name="British", _shortName = "UK" },
            new StockInfo() {id=11, name="Bulgarian"},
            new StockInfo() {id=12, name="Cameroonian"},
            new StockInfo() {id=13, name="Canadian", _shortName = "CAN"},
            new StockInfo() {id=14, name="Chilean" },
            new StockInfo() {id=15, name="Chinese", _shortName = "CN" },
            new StockInfo() {id=16, name="Colombian"},
            new StockInfo() {id=17, name="Costa Rican", _shortName = "CR" },
            new StockInfo() {id=18, name="Croatian", _shortName = "CRO"},
            new StockInfo() {id=19, name="Cypriot"},
            new StockInfo() {id=20, name="Czech"},
            new StockInfo() {id=21, name="Danish"},
            new StockInfo() {id=22, name="Dutch", _shortName = "NL"},
            new StockInfo() {id=23, name="Ecuadorian"},
            new StockInfo() {id=24, name="English", _shortName = "UK"},
            new StockInfo() {id=25, name="Emirian"},
            new StockInfo() {id=26, name="Estonian"},
            new StockInfo() {id=27, name="Finnish"},
            new StockInfo() {id=28, name="French"},
            new StockInfo() {id=29, name="German"},
            new StockInfo() {id=30, name="Ghanaian"},
            new StockInfo() {id=31, name="Greek"},
            new StockInfo() {id=32, name="Guatemalan"},
            new StockInfo() {id=33, name="Honduran"},
            new StockInfo() {id=34, name="Hong Konger", _shortName = "HK"},
            new StockInfo() {id=35, name="Hungarian"},
            new StockInfo() {id=36, name="Icelander"},
            new StockInfo() {id=37, name="Indian" , _shortName = "IDN"},
            new StockInfo() {id=38, name="Indonesian", _shortName = "IND"},
            new StockInfo() {id=39, name="Irish"},
            new StockInfo() {id=40, name="Israeli"},
            new StockInfo() {id=41, name="Italian"},
            new StockInfo() {id=42, name="Jamaican"},
            new StockInfo() {id=43, name="Japanese"},
            new StockInfo() {id=44, name="Jordanian"},
            new StockInfo() {id=45, name="Kuwaiti"},
            new StockInfo() {id=46, name="Latvian"},
            new StockInfo() {id=47, name="Lebanese"},
            new StockInfo() {id=48, name="Lithuanian"},
            new StockInfo() {id=49, name="Luxembourger"},
            new StockInfo() {id=50, name="Malaysian", _shortName = "MAY"},
            new StockInfo() {id=51, name="Maltese", _shortName = "MLT"},
            new StockInfo() {id=52, name="Mexican"},
            new StockInfo() {id=53, name="Monegasque"},
            new StockInfo() {id=54, name="New Zealander", _shortName = "NZ"},
            new StockInfo() {id=55, name="Nicaraguan"},
            new StockInfo() {id=56, name="North Korean", _shortName = "N-K"},
            new StockInfo() {id=57, name="Northern Irish", _shortName = "NIR"},
            new StockInfo() {id=58, name="Norwegian"},
            new StockInfo() {id=59, name="Omani"},
            new StockInfo() {id=60, name="Pakistani"},
            new StockInfo() {id=61, name="Panamanian"},
            new StockInfo() {id=62, name="Paraguayan"},
            new StockInfo() {id=63, name="Peruvian"},
            new StockInfo() {id=64, name="Polish"},
            new StockInfo() {id=65, name="Portuguese"},
            new StockInfo() {id=66, name="Qatari"},
            new StockInfo() {id=67, name="Romanian"},
            new StockInfo() {id=68, name="Russian"},
            new StockInfo() {id=69, name="Salvadoran"},
            new StockInfo() {id=70, name="Saudi", _shortName = "SAU"},
            new StockInfo() {id=71, name="Scottish"},
            new StockInfo() {id=72, name="Serbian"},
            new StockInfo() {id=73, name="Singaporean"},
            new StockInfo() {id=74, name="Slovakian"},
            new StockInfo() {id=75, name="Slovenian"},
            new StockInfo() {id=76, name="South Korean", _shortName = "KOR"},
            new StockInfo() {id=77, name="South African", _shortName = "SAF"},
            new StockInfo() {id=78, name="Spanish"},
            new StockInfo() {id=79, name="Swedish"},
            new StockInfo() {id=80, name="Swiss"},
            new StockInfo() {id=81, name="Thai"},
            new StockInfo() {id=82, name="Turkish"},
            new StockInfo() {id=83, name="Uruguayan"},
            new StockInfo() {id=84, name="Ukrainian"},
            new StockInfo() {id=85, name="Venezuelan"},
            new StockInfo() {id=86, name="Welsh"},
            new StockInfo() {id=87, name="Barbadian"},
            new StockInfo() {id=88, name="Vietnamese" },
        };
        public static List<StockInfo> DriverList = new List<StockInfo>()
        {
            new StockInfo() {id=0, name="Carlos Sainz"},
            new StockInfo() {id=1, name="Daniil Kvyat"},
            new StockInfo() {id=2, name="Daniel Ricciardo"},
            new StockInfo() {id=6, name="Kimi Räikkönen"},
            new StockInfo() {id=7, name="Lewis Hamilton"},
            new StockInfo() {id=9, name="Max Verstappen"},
            new StockInfo() {id=10, name="Nico Hulkenburg"},
            new StockInfo() {id=11, name="Kevin Magnussen"},
            new StockInfo() {id=12, name="Romain Grosjean"},
            new StockInfo() {id=13, name="Sebastian Vettel"},
            new StockInfo() {id=14, name="Sergio Perez"},
            new StockInfo() {id=15, name="Valtteri Bottas"},
            new StockInfo() {id=17, name="Esteban Ocon"},
            new StockInfo() {id=19, name="Lance Stroll"},
            new StockInfo() {id=20, name="Arron Barnes"},
            new StockInfo() {id=21, name="Martin Giles"},
            new StockInfo() {id=22, name="Alex Murray"},
            new StockInfo() {id=23, name="Lucas Roth"},
            new StockInfo() {id=24, name="Igor Correia"},
            new StockInfo() {id=25, name="Sophie Levasseur"},
            new StockInfo() {id=26, name="Jonas Schiffer"},
            new StockInfo() {id=27, name="Alain Forest"},
            new StockInfo() {id=28, name="Jay Letourneau"},
            new StockInfo() {id=29, name="Esto Saari"},
            new StockInfo() {id=30, name="Yasar Atiyeh"},
            new StockInfo() {id=31, name="Callisto Calabresi"},
            new StockInfo() {id=32, name="Naota Izum"},
            new StockInfo() {id=33, name="Howard Clarke"},
            new StockInfo() {id=34, name="Wilheim Kaufmann"},
            new StockInfo() {id=35, name="Marie Laursen"},
            new StockInfo() {id=36, name="Flavio Nieves"},
            new StockInfo() {id=37, name="Peter Belousov"},
            new StockInfo() {id=38, name="Klimek Michalski"},
            new StockInfo() {id=39, name="Santiago Moreno"},
            new StockInfo() {id=40, name="Benjamin Coppens"},
            new StockInfo() {id=41, name="Noah Visser"},
            new StockInfo() {id=42, name="Gert Waldmuller"},
            new StockInfo() {id=43, name="Julian Quesada"},
            new StockInfo() {id=44, name="Daniel Jones"},
            new StockInfo() {id=45, name="Artem Markelov"},
            new StockInfo() {id=46, name="Tadasuke Makino"},
            new StockInfo() {id=47, name="Sean Gelael"},
            new StockInfo() {id=48, name="Nyck De Vries"},
            new StockInfo() {id=49, name="Jack Aitken"},
            new StockInfo() {id=50, name="George Russell"},
            new StockInfo() {id=51, name="Maximilian Günther"},
            new StockInfo() {id=52, name="Nirei Fukuzumi"},
            new StockInfo() {id=53, name="Luca Ghiotto"},
            new StockInfo() {id=54, name="Lando Norris"},
            new StockInfo() {id=55, name="Sérgio Sette Câmara"},
            new StockInfo() {id=56, name="Louis Delétraz"},
            new StockInfo() {id=57, name="Antonio Fuoco"},
            new StockInfo() {id=58, name="Charles Leclerc"},
            new StockInfo() {id=59, name="Pierre Gasly"},
            new StockInfo() {id=62, name="Alexander Albon"},
            new StockInfo() {id=63, name="Nicholas Latifi"},
            new StockInfo() {id=64, name="Dorian Boccolacci"},
            new StockInfo() {id=65, name="Niko Kari"},
            new StockInfo() {id=66, name="Roberto Merhi"},
            new StockInfo() {id=67, name="Arjun Maini"},
            new StockInfo() {id=68, name="Alessio Lorandi"},
            new StockInfo() {id=69, name="Ruben Meijer"},
            new StockInfo() {id=70, name="Rashid Nair"},
            new StockInfo() {id=71, name="Jack Tremblay"},
            new StockInfo() {id=74, name="Antonio Giovinazzi"},
            new StockInfo() {id=75, name="Robert Kubica"},
            new StockInfo() {id=78, name="Nobuharu Matsushita"},
            new StockInfo() {id=79, name="Nikita Mazepin"},
            new StockInfo() {id=80, name="Guanya Zhou"},
            new StockInfo() {id=81, name="Mick Schumacher"},
            new StockInfo() {id=82, name="Callum Ilott"},
            new StockInfo() {id=83, name="Juan Manuel Correa"},
            new StockInfo() {id=84, name="Jordan King"},
            new StockInfo() {id=85, name="Mahaveer Raghunathan"},
            new StockInfo() {id=86, name="Tatiana Calderon"},
            new StockInfo() {id=87, name="Anthoine Hubert"},
            new StockInfo() {id=88, name="Guiliano Alesi"},
            new StockInfo() {id=89, name="Ralph Boschung"},

        };
        public static List<StockInfo> TrackList = new List<StockInfo>()
        {
            new StockInfo() {id=0, name="Melbourne"},
            new StockInfo() {id=1, name="Paul Ricard"},
            new StockInfo() {id=2, name="Shanghai"},
            new StockInfo() {id=3, name="Sakhir (Bahrain)"},
            new StockInfo() {id=4, name="Catalunya"},
            new StockInfo() {id=5, name="Monaco"},
            new StockInfo() {id=6, name="Montreal"},
            new StockInfo() {id=7, name="Silverstone"},
            new StockInfo() {id=8, name="Hockenheim"},
            new StockInfo() {id=9, name="Hungaroring"},
            new StockInfo() {id=10, name="Spa"},
            new StockInfo() {id=11, name="Monza"},
            new StockInfo() {id=12, name="Singapore"},
            new StockInfo() {id=13, name="Suzuka"},
            new StockInfo() {id=14, name="Abu Dhabi"},
            new StockInfo() {id=15, name="Texas"},
            new StockInfo() {id=16, name="Brazil"},
            new StockInfo() {id=17, name="Austria"},
            new StockInfo() {id=18, name="Sochi"},
            new StockInfo() {id=19, name="Mexico"},
            new StockInfo() {id=20, name="Baku (Azerbaijan)"},
            new StockInfo() {id=21, name="Sakhir Short"},
            new StockInfo() {id=22, name="Silverstone Short"},
            new StockInfo() {id=23, name="Texas Short"},
            new StockInfo() {id=24, name="Suzuka Short"},
            new StockInfo() {id=25, name="Hanoi"},
            new StockInfo() {id=26, name="Zandvoort"},

        };
        public static List<StockInfo> SurfaceList = new List<StockInfo>()
        {
            new StockInfo() {id=0, name="Tarmac"},
            new StockInfo() {id=1, name="Rumble strip"},
            new StockInfo() {id=2, name="Concrete"},
            new StockInfo() {id=3, name="Rock"},
            new StockInfo() {id=4, name="Gravel"},
            new StockInfo() {id=5, name="Mud"},
            new StockInfo() {id=6, name="Sand"},
            new StockInfo() {id=7, name="Grass"},
            new StockInfo() {id=8, name="Water"},
            new StockInfo() {id=9, name="Cobblestone"},
            new StockInfo() {id=10, name="Metal"},
            new StockInfo() {id=11, name="Ridged"}
        };
        public static List<StockInfo> PenaltyList = new List<StockInfo>()
        {
            new StockInfo() {id=0, name="Drive through"},
            new StockInfo() {id=1, name="Stop Go"},
            new StockInfo() {id=2, name="Grid penalty"},
            new StockInfo() {id=3, name="Penalty reminder"},
            new StockInfo() {id=4, name="Time penalty"},
            new StockInfo() {id=5, name="Warning"},
            new StockInfo() {id=6, name="Disqualified"},
            new StockInfo() {id=7, name="Removed from formation lap"},
            new StockInfo() {id=8, name="Parked too long timer"},
            new StockInfo() {id=9, name="Tyre regulations"},
            new StockInfo() {id=10, name="This lap invalidated"},
            new StockInfo() {id=11, name="This and next lap invalidated"},
            new StockInfo() {id=12, name="This lap invalidated without reason"},
            new StockInfo() {id=13, name="This and next lap invalidated without reason"},
            new StockInfo() {id=14, name="This and previous lap invalidated"},
            new StockInfo() {id=15, name="This and previous lap invalidated without reason"},
            new StockInfo() {id=16, name="Retired"},
            new StockInfo() {id=17, name="Black flag timer"}
        };
        public static List<StockInfo> InfringmentList = new List<StockInfo>()
        {
            new StockInfo() {id=0, name="Blocking by slow driving"},
            new StockInfo() {id=1, name="Blocking by wrong way driving"},
            new StockInfo() {id=2, name="Reversing off the start line"},
            new StockInfo() {id=3, name="Big Collision"},
            new StockInfo() {id=4, name="Small Collision"},
            new StockInfo() {id=5, name="Collision failed to hand back position single"},
            new StockInfo() {id=6, name="Collision failed to hand back position multiple"},
            new StockInfo() {id=7, name="Corner cutting gained time"},
            new StockInfo() {id=8, name="Corner cutting overtake single"},
            new StockInfo() {id=9, name="Corner cutting overtake multiple"},
            new StockInfo() {id=10, name="Crossed pit exit lane"},
            new StockInfo() {id=11, name="Ignoring blue flags"},
            new StockInfo() {id=12, name="Ignoring yellow flags"},
            new StockInfo() {id=13, name="Ignoring drive through"},
            new StockInfo() {id=14, name="Too many drive throughs"},
            new StockInfo() {id=15, name="Drive through reminder serve within n laps"},
            new StockInfo() {id=16, name="Drive through reminder serve this lap"},
            new StockInfo() {id=17, name="Pit lane speeding"},
            new StockInfo() {id=18, name="Parked for too long"},
            new StockInfo() {id=19, name="Ignoring tyre regulations"},
            new StockInfo() {id=20, name="Too many penalties"},
            new StockInfo() {id=21, name="Multiple warnings"},
            new StockInfo() {id=22, name="Approaching disqualification"},
            new StockInfo() {id=23, name="Tyre regulations select single"},
            new StockInfo() {id=24, name="Tyre regulations select multiple"},
            new StockInfo() {id=25, name="Lap invalidated corner cutting"},
            new StockInfo() {id=26, name="Lap invalidated running wide"},
            new StockInfo() {id=27, name="Corner cutting ran wide gained time minor"},
            new StockInfo() {id=28, name="Corner cutting ran wide gained time significant"},
            new StockInfo() {id=29, name="Corner cutting ran wide gained time extreme"},
            new StockInfo() {id=30, name="Lap invalidated wall riding"},
            new StockInfo() {id=31, name="Lap invalidated flashback used"},
            new StockInfo() {id=32, name="Lap invalidated reset to track"},
            new StockInfo() {id=33, name="Blocking the pitlane"},
            new StockInfo() {id=34, name="Jump start"},
            new StockInfo() {id=35, name="Safety car to car collision"},
            new StockInfo() {id=36, name="Safety car illegal overtake"},
            new StockInfo() {id=37, name="Safety car exceeding allowed pace"},
            new StockInfo() {id=38, name="Virtual safety car exceeding allowed pace"},
            new StockInfo() {id=39, name="Formation lap below allowed speed"},
            new StockInfo() {id=40, name="Retired mechanical failure"},
            new StockInfo() {id=41, name="Retired terminally damaged"},
            new StockInfo() {id=42, name="Safety car falling too far back"},
            new StockInfo() {id=43, name="Black flag timer"},
            new StockInfo() {id=44, name="Unserved stop go penalty"},
            new StockInfo() {id=45, name="Unserved drive through penalty"},
            new StockInfo() {id=46, name="Engine component change"},
            new StockInfo() {id=47, name="Gearbox change"},
            new StockInfo() {id=48, name="League grid penalty"},
            new StockInfo() {id=49, name="Retry penalty"},
            new StockInfo() {id=50, name="Illegal time gain"},
            new StockInfo() {id=51, name="Mandatory pitstop"},

        };
        public static List<StockInfo> SessionTypeList = new List<StockInfo>()
        {
            new StockInfo() {id=0, name="Unknown"},
            new StockInfo() {id=1, name="Practice1"},
            new StockInfo() {id=2, name="Practice2"},
            new StockInfo() {id=3, name="Practice3"},
            new StockInfo() {id=4, name="Short P"},
            new StockInfo() {id=5, name="Q1"},
            new StockInfo() {id=6, name="Q2"},
            new StockInfo() {id=7, name="Q3"},
            new StockInfo() {id=8, name="Short Qualy"},
            new StockInfo() {id=9, name="OSQ"},
            new StockInfo() {id=10, name="Race"},
            new StockInfo() {id=11, name="Race 2"},
            new StockInfo() {id=12, name="Time Trial"},

        };
        public static string GetName(StockType t, int id)
        {
            return Get(t, id).name;
        }
        public static string GetShortName(StockType t, int id)
        {
            return Get(t, id).shortName;
        }

        public static StockInfo Get(StockType stockType, int id)
        {
            switch (stockType)
            {
                case StockType.stTeam:
                    return TeamList.FirstOrDefault(t => t.id == id);
                case StockType.stCountry:
                    return CountryList.FirstOrDefault(t => t.id == id);
                case StockType.stDriver:
                    return DriverList.FirstOrDefault(t => t.id == id);
                case StockType.stTrack:
                    return TrackList.FirstOrDefault(t => t.id == id);
                case StockType.stSurface:
                    return SurfaceList.FirstOrDefault(t => t.id == id);
                case StockType.stPenalty:
                    return PenaltyList.FirstOrDefault(t => t.id == id);
                case StockType.stInfringment:
                    return InfringmentList.FirstOrDefault(t => t.id == id);
                case StockType.stSessionType:
                    return SessionTypeList.FirstOrDefault(t => t.id == id);
                default:
                    return null;
            }
        }
    }
}
