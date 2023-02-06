﻿using System.ComponentModel.DataAnnotations;

// ReSharper disable UnusedMember.Global

namespace NiotTelegramBot.ModelzAndUtils.Enums;

[Serializable]
public enum Emoji
{
    [Display(Name = "")] None,
    [Display(Name = "🚪")] Door,
    [Display(Name = "💠")] DiamondWithDot,
    [Display(Name = "🔳")] WhiteSquareButton,
    [Display(Name = "🔲")] BlackSquareButton,
    [Display(Name = "💼")] Briefcase,
    [Display(Name = "📁")] FileFolder,
    [Display(Name = "📂")] OpenFileFolder,
    [Display(Name = "🗂")] CardIndexDividers,
    [Display(Name = "🗄")] FileCabinet,
    [Display(Name = "⚖")] BalanceScale,
    [Display(Name = "🦯")] WhiteCane,
    [Display(Name = "🔗")] Link,
    [Display(Name = "⛓")] Chains,
    [Display(Name = "👯")] PeopleWithBunnyEars,
    [Display(Name = "🪝")] Hook,
    [Display(Name = "🧰")] Toolbox,
    [Display(Name = "🧲")] Magnet,
    [Display(Name = "🪜")] Ladder,
    [Display(Name = "🧪")] TestTube,
    [Display(Name = "🔬")] Microscope,
    [Display(Name = "🔭")] Telescope,
    [Display(Name = "📡")] SatelliteAntenna,
    [Display(Name = "🩹")] AdhesiveBandage,
    [Display(Name = "🩼")] Crutch,
    [Display(Name = "🩺")] Stethoscope,
    [Display(Name = "🛋")] CouchAndLamp,
    [Display(Name = "🪑")] Chair,
    [Display(Name = "🚽")] Toilet,
    [Display(Name = "🪠")] Plunger,
    [Display(Name = "🚿")] Shower,
    [Display(Name = "🛁")] Bathtub,
    [Display(Name = "🛀")] PersonTakingBath,
    [Display(Name = "🛌")] PersonInBed,
    [Display(Name = "🐶")] DogFace,
    [Display(Name = "🦊")] Fox,
    [Display(Name = "🦧")] Orangutan,
    [Display(Name = "🐵")] MonkeyFace,
    [Display(Name = "🦱")] CurlyHair,
    [Display(Name = "🐩")] Poodle,
    [Display(Name = "🐕‍🦺")] ServiceDog,
    [Display(Name = "🦮")] GuideDog,
    [Display(Name = "🐺")] Wolf,
    [Display(Name = "🦝")] Raccoon,
    [Display(Name = "🐈")] Cat,
    [Display(Name = "🐱")] CatFace,
    [Display(Name = "🐅")] Tiger,
    [Display(Name = "🐯")] TigerFace,
    [Display(Name = "🐆")] Leopard,
    [Display(Name = "🦒")] Giraffe,
    [Display(Name = "🦛")] Hippopotamus,
    [Display(Name = "🦁")] Lion,
    [Display(Name = "🦄")] Unicorn,
    [Display(Name = "🐪")] Camel,
    [Display(Name = "🐫")] TwoHumpCamel,
    [Display(Name = "🐐")] Goat,
    [Display(Name = "🐑")] Ewe,
    [Display(Name = "🐏")] Ram,
    [Display(Name = "🐽")] PigNose,
    [Display(Name = "🐗")] Boar,
    [Display(Name = "🐖")] Pig,
    [Display(Name = "🐷")] PigFace,
    [Display(Name = "🐄")] Cow,
    [Display(Name = "🐃")] WaterBuffalo,
    [Display(Name = "🐂")] Ox,
    [Display(Name = "🐮")] CowFace,
    [Display(Name = "🦬")] Bison,
    [Display(Name = "🦓")] Zebra,
    [Display(Name = "🐁")] Mouse,
    [Display(Name = "🐰")] RabbitFace,
    [Display(Name = "🐇")] Rabbit,
    [Display(Name = "🪶")] Feather,
    [Display(Name = "🐸")] Frog,
    [Display(Name = "🦎")] Lizard,
    [Display(Name = "🐲")] DragonFace,
    [Display(Name = "🐠")] TropicalFish,
    [Display(Name = "🐡")] Blowfish,
    [Display(Name = "🦋")] Butterfly,
    [Display(Name = "🐛")] Bug,
    [Display(Name = "🦗")] Cricket,
    [Display(Name = "🪳")] Cockroach,
    [Display(Name = "🕷")] Spider,
    [Display(Name = "🦂")] Scorpion,
    [Display(Name = "🦟")] Mosquito,
    [Display(Name = "🪰")] Fly,
    [Display(Name = "🪱")] Worm,
    [Display(Name = "🦠")] Microbe,
    [Display(Name = "🐿")] Chipmunk,
    [Display(Name = "🦇")] Bat,
    [Display(Name = "🐻")] Bear,
    [Display(Name = "🐨")] Koala,
    [Display(Name = "🦥")] Sloth,
    [Display(Name = "🦘")] Kangaroo,
    [Display(Name = "🦦")] Otter,
    [Display(Name = "🦡")] Badger,
    [Display(Name = "🐘")] Elephant,
    [Display(Name = "🐭")] MouseFace,
    [Display(Name = "🐀")] Rat,
    [Display(Name = "🐹")] Hamster,
    [Display(Name = "🦔")] Hedgehog,
    [Display(Name = "🦙")] Llama,
    [Display(Name = "🦫")] Beaver,
    [Display(Name = "🐼")] Panda,
    [Display(Name = "🦨")] Skunk,
    [Display(Name = "🐈‍⬛")] BlackCat,
    [Display(Name = "🐦")] Bird,
    [Display(Name = "🐧")] Penguin,
    [Display(Name = "🦆")] Duck,
    [Display(Name = "🦢")] Swan,
    [Display(Name = "🦩")] Flamingo,
    [Display(Name = "🦚")] Peacock,
    [Display(Name = "🦃")] Turkey,
    [Display(Name = "🐤")] BabyChick,
    [Display(Name = "🐓")] Rooster,
    [Display(Name = "🐣")] HatchingChick,
    [Display(Name = "🐥")] FrontFacingBabyChick,
    [Display(Name = "🐔")] Chicken,
    [Display(Name = "🦉")] Owl,
    [Display(Name = "🦜")] Parrot,
    [Display(Name = "🐊")] Crocodile,
    [Display(Name = "🐢")] Turtle,
    [Display(Name = "🐍")] Snake,
    [Display(Name = "🐉")] Dragon,
    [Display(Name = "🦕")] Sauropod,
    [Display(Name = "🦭")] Seal,
    [Display(Name = "🌺")] Hibiscus,
    [Display(Name = "🏵")] Rosette,
    [Display(Name = "💮")] WhiteFlower,
    [Display(Name = "🌸")] CherryBlossom,
    [Display(Name = "💐")] Bouquet,
    [Display(Name = "🌼")] Blossom,
    [Display(Name = "🌷")] Tulip,
    [Display(Name = "🌱")] Seedling,
    [Display(Name = "☘")] Shamrock,
    [Display(Name = "🍀")] FourLeafClover,
    [Display(Name = "🪴")] PottedPlant,
    [Display(Name = "🌾")] SheafOfRice,
    [Display(Name = "🌹")] Rose,
    [Display(Name = "🥀")] WiltedFlower,
    [Display(Name = "🍁")] MapleLeaf,
    [Display(Name = "🪺")] NestWithEggs,
    [Display(Name = "🪹")] EmptyNest,
    [Display(Name = "🍇")] Grapes,
    [Display(Name = "🍉")] Watermelon,
    [Display(Name = "🍋")] Lemon,
    [Display(Name = "🍊")] Tangerine,
    [Display(Name = "🍈")] Melon,
    [Display(Name = "🍌")] Banana,
    [Display(Name = "🍍")] Pineapple,
    [Display(Name = "🍎")] RedApple,
    [Display(Name = "🍏")] GreenApple,
    [Display(Name = "🍒")] Cherries,
    [Display(Name = "🍑")] Peach,
    [Display(Name = "🍐")] Pear,
    [Display(Name = "🥭")] Mango,
    [Display(Name = "🍓")] Strawberry,
    [Display(Name = "🫒")] Olive,
    [Display(Name = "🌰")] Chestnut,
    [Display(Name = "🥜")] Peanuts,
    [Display(Name = "🥨")] Pretzel,
    [Display(Name = "🥞")] Pancakes,
    [Display(Name = "🥯")] Bagel,
    [Display(Name = "🫓")] Flatbread,
    [Display(Name = "🍖")] MeatOnBone,
    [Display(Name = "🧀")] CheeseWedge,
    [Display(Name = "🍟")] FrenchFries,
    [Display(Name = "🍕")] Pizza,
    [Display(Name = "🧇")] Waffle,
    [Display(Name = "🥚")] Egg,
    [Display(Name = "🍳")] Cooking,
    [Display(Name = "🥘")] ShallowPanOfFood,
    [Display(Name = "🍲")] PotOfFood,
    [Display(Name = "🧈")] Butter,
    [Display(Name = "🧂")] Salt,
    [Display(Name = "🥫")] CannedFood,
    [Display(Name = "🍣")] Sushi,
    [Display(Name = "🥟")] Dumpling,
    [Display(Name = "🌮")] Taco,
    [Display(Name = "🍦")] SoftIceCream,
    [Display(Name = "🍧")] ShavedIce,
    [Display(Name = "🍨")] IceCream,
    [Display(Name = "🍩")] Doughnut,
    [Display(Name = "🍪")] Cookie,
    [Display(Name = "🥠")] FortuneCookie,
    [Display(Name = "🍤")] FriedShrimp,
    [Display(Name = "🍰")] Shortcake,
    [Display(Name = "🧁")] Cupcake,
    [Display(Name = "🥧")] Pie,
    [Display(Name = "🫘")] Beans,
    [Display(Name = "🧅")] Onion,
    [Display(Name = "🧄")] Garlic,
    [Display(Name = "🥬")] LeafyGreen,
    [Display(Name = "🥒")] Cucumber,
    [Display(Name = "🌶")] HotPepper,
    [Display(Name = "🌽")] EarOfCorn,
    [Display(Name = "🍆")] Eggplant,
    [Display(Name = "🥑")] Avocado,
    [Display(Name = "🥝")] KiwiFruit,
    [Display(Name = "🫐")] Blueberries,
    [Display(Name = "🍫")] ChocolateBar,
    [Display(Name = "🍬")] Candy,
    [Display(Name = "🍭")] Lollipop,
    [Display(Name = "🍮")] Custard,
    [Display(Name = "🍯")] HoneyPot,
    [Display(Name = "🥛")] GlassOfMilk,
    [Display(Name = "🥤")] CupWithStraw,
    [Display(Name = "🥗")] GreenSalad,
    [Display(Name = "🌯")] Burrito,
    [Display(Name = "🥪")] Sandwich,
    [Display(Name = "🌭")] HotDog,
    [Display(Name = "🍔")] Hamburger,
    [Display(Name = "🥩")] CutOfMeat,
    [Display(Name = "🥓")] Bacon,
    [Display(Name = "🍗")] PoultryLeg,
    [Display(Name = "🥖")] BaguetteBread,
    [Display(Name = "🥐")] Croissant,
    [Display(Name = "🧃")] BeverageBox,
    [Display(Name = "🧋")] BubbleTea,
    [Display(Name = "🥃")] TumblerGlass,
    [Display(Name = "🥂")] ClinkingGlasses,
    [Display(Name = "🍻")] ClinkingBeerMugs,
    [Display(Name = "🍺")] BeerMug,
    [Display(Name = "🍹")] TropicalDrink,
    [Display(Name = "🍸")] CocktailGlass,
    [Display(Name = "🍷")] WineGlass,
    [Display(Name = "🍾")] BottleWithPoppingCork,
    [Display(Name = "🍼")] BabyBottle,
    [Display(Name = "🎂")] BirthdayCake,
    [Display(Name = "🥡")] TakeoutBox,
    [Display(Name = "🍜")] SteamingBowl,
    [Display(Name = "🍝")] Spaghetti,
    [Display(Name = "🍛")] CurryRice,
    [Display(Name = "🍚")] CookedRice,
    [Display(Name = "🧉")] Mate,
    [Display(Name = "🥢")] Chopsticks,
    [Display(Name = "🍴")] ForkAndKnife,
    [Display(Name = "🍽")] ForkAndKnifeWithPlate,
    [Display(Name = "🥄")] Spoon,
    [Display(Name = "🏺")] Amphora,
    [Display(Name = "🏚")] DerelictHouse,
    [Display(Name = "🏤")] PostOffice,
    [Display(Name = "🏪")] ConvenienceStore,
    [Display(Name = "🍅")] Tomato,
    [Display(Name = "🥥")] Coconut,
    [Display(Name = "🥔")] Potato,
    [Display(Name = "🥕")] Carrot,
    [Display(Name = "🥦")] Broccoli,
    [Display(Name = "🦞")] Lobster,
    [Display(Name = "🦀")] Crab,
    [Display(Name = "🦑")] Squid,
    [Display(Name = "🦐")] Shrimp,
    [Display(Name = "🦪")] Oyster,
    [Display(Name = "☕")] HotBeverage,
    [Display(Name = "🫖")] Teapot,
    [Display(Name = "🫗")] PouringLiquid,
    [Display(Name = "🏰")] Castle,
    [Display(Name = "🏬")] DepartmentStore,
    [Display(Name = "🗼")] TokyoTower,
    [Display(Name = "🏫")] School,
    [Display(Name = "⛲")] Fountain,
    [Display(Name = "🏦")] Bank,
    [Display(Name = "🏨")] Hotel,
    [Display(Name = "🏥")] Hospital,
    [Display(Name = "🏡")] HouseWithGarden,
    [Display(Name = "🪵")] Wood,
    [Display(Name = "🪨")] Rock,
    [Display(Name = "🏟")] Stadium,
    [Display(Name = "⛺")] Tent,
    [Display(Name = "🏙")] Cityscape,
    [Display(Name = "🌄")] SunriseOverMountains,
    [Display(Name = "🌅")] Sunrise,
    [Display(Name = "🌉")] BridgeAtNight,
    [Display(Name = "🌇")] Sunset,
    [Display(Name = "🎠")] CarouselHorse,
    [Display(Name = "🛝")] PlaygroundSlide,
    [Display(Name = "🎡")] FerrisWheel,
    [Display(Name = "🎢")] RollerCoaster,
    [Display(Name = "💈")] BarberPole,
    [Display(Name = "🌆")] CityscapeAtDusk,
    [Display(Name = "🌃")] NightWithStars,
    [Display(Name = "🌁")] Foggy,
    [Display(Name = "🛖")] Hut,
    [Display(Name = "🎪")] CircusTent,
    [Display(Name = "🚤")] Speedboat,
    [Display(Name = "⛵")] Sailboat,
    [Display(Name = "🛫")] AirplaneDeparture,
    [Display(Name = "🛬")] AirplaneArrival,
    [Display(Name = "💺")] Seat,
    [Display(Name = "🚁")] Helicopter,
    [Display(Name = "🧳")] Luggage,
    [Display(Name = "⌛")] HourGlassDone,
    [Display(Name = "⏲")] TimerClock,
    [Display(Name = "⏳")] HourGlassNotDone,
    [Display(Name = "🌒")] WaxingCrescentMoon,
    [Display(Name = "🌛")] FirstQuarterMoonFace,
    [Display(Name = "🌞")] SunWithFace,
    [Display(Name = "🌌")] MilkyWay,
    [Display(Name = "🎍")] PineDecoration,
    [Display(Name = "🧧")] RedEnvelope,
    [Display(Name = "🎐")] WindChime,
    [Display(Name = "🎎")] JapaneseDolls,
    [Display(Name = "⚽")] SoccerBall,
    [Display(Name = "🏀")] Basketball,
    [Display(Name = "🎾")] Tennis,
    [Display(Name = "🎳")] Bowling,
    [Display(Name = "🏒")] IceHockey,
    [Display(Name = "🏓")] PingPong,
    [Display(Name = "🥅")] GoalNet,
    [Display(Name = "🎱")] PoolEightBall,
    [Display(Name = "⛸")] IceSkate,
    [Display(Name = "⌚")] Watch,
    [Display(Name = "🌠")] ShootingStar,
    [Display(Name = "🌟")] GlowingStar,
    [Display(Name = "🚄")] HighSpeedTrain,
    [Display(Name = "🚅")] BulletTrain,
    [Display(Name = "🚆")] Train,
    [Display(Name = "🚇")] Metro,
    [Display(Name = "🚉")] Station,
    [Display(Name = "🚐")] Minibus,
    [Display(Name = "🚙")] SportUtilityVehicle,
    [Display(Name = "🏍")] Motorcycle,
    [Display(Name = "🚲")] Bicycle,
    [Display(Name = "🛴")] KickScooter,
    [Display(Name = "🛹")] Skateboard,
    [Display(Name = "🚏")] BusStop,
    [Display(Name = "🛣")] Motorway,
    [Display(Name = "🛤")] RailwayTrack,
    [Display(Name = "🚨")] PoliceCarLight,
    [Display(Name = "🚥")] HorizontalTrafficLight,
    [Display(Name = "🛳")] PassengerShip,
    [Display(Name = "⛴")] Ferry,
    [Display(Name = "🛥")] MotorBoat,
    [Display(Name = "🛩")] SmallAirplanet,
    [Display(Name = "☁")] Cloud,
    [Display(Name = "🐾")] PawPrints,
    [Display(Name = "🕊")] Dove,
    [Display(Name = "🦅")] Eagle,
    [Display(Name = "🦖")] Rex,
    [Display(Name = "🐋")] Whale,
    [Display(Name = "🐳")] SpoutingWhale,
    [Display(Name = "🐬")] Dolphin,
    [Display(Name = "🐟")] Fish,
    [Display(Name = "🦈")] Shark,
    [Display(Name = "🐙")] Octopus,
    [Display(Name = "🐚")] SpiralShell,
    [Display(Name = "🐌")] Snail,
    [Display(Name = "🐝")] Honeybee,
    [Display(Name = "🐜")] Ant,
    [Display(Name = "🪲")] Beetle,
    [Display(Name = "🐞")] LadyBeetle,
    [Display(Name = "🕸")] SpiderWeb,
    [Display(Name = "🌻")] Sunflower,
    [Display(Name = "🌲")] EvergreenTree,
    [Display(Name = "🌳")] DeciduousTree,
    [Display(Name = "🌴")] PalmTree,
    [Display(Name = "🌵")] Cactus,
    [Display(Name = "🌿")] Herb,
    [Display(Name = "🍄")] Mushroom,
    [Display(Name = "🍞")] Bread,
    [Display(Name = "🥣")] BowlWithSpoon,
    [Display(Name = "🧊")] Ice,
    [Display(Name = "🔪")] KitchenKnife,
    [Display(Name = "🗺")] WorldMap,
    [Display(Name = "🧭")] Compass,
    [Display(Name = "🏔")] SnowCappedMountain,
    [Display(Name = "⛰")] Mountain,
    [Display(Name = "🌋")] Volcano,
    [Display(Name = "🗻")] MountFuji,
    [Display(Name = "🏕")] Camping,
    [Display(Name = "🏖")] BeachWithUmbrella,
    [Display(Name = "🏜")] Desert,
    [Display(Name = "🏝")] DesertIsland,
    [Display(Name = "🏞")] NationalPark,
    [Display(Name = "🏛")] ClassicalBuilding,
    [Display(Name = "🏗")] BuildingConstruction,
    [Display(Name = "🧱")] Brick,
    [Display(Name = "🏘")] Houses,
    [Display(Name = "🏠")] House,
    [Display(Name = "🏢")] OfficeBuilding,
    [Display(Name = "🏭")] Factory,
    [Display(Name = "♨")] HotSprings,
    [Display(Name = "🚂")] Locomotive,
    [Display(Name = "🚌")] Bus,
    [Display(Name = "🚑")] Ambulance,
    [Display(Name = "🚒")] FireEngine,
    [Display(Name = "🚓")] PoliceCar,
    [Display(Name = "🚕")] Taxi,
    [Display(Name = "🚗")] Automobile,
    [Display(Name = "🚚")] DeliveryTruck,
    [Display(Name = "🚜")] Tractor,
    [Display(Name = "🏎")] RacingCar,
    [Display(Name = "🛢")] OilDrum,
    [Display(Name = "⛽")] FuelPump,
    [Display(Name = "🛞")] Wheel,
    [Display(Name = "🚦")] VerticalTrafficLight,
    [Display(Name = "⚓")] Anchor,
    [Display(Name = "🚢")] Ship,
    [Display(Name = "✈")] Airplane,
    [Display(Name = "🛰")] Satellite,
    [Display(Name = "🧤")] Gloves,

    [Display(Name = "🪤")] MouseTrap,
    [Display(Name = "🪒")] Razor,
    [Display(Name = "🧴")] LotionBottle,
    [Display(Name = "🧷")] SafetyPin,
    [Display(Name = "🧹")] Broom,
    [Display(Name = "🧻")] RollOfPaper,
    [Display(Name = "🪣")] Bucket,
    [Display(Name = "🧼")] Soap,
    [Display(Name = "🫧")] Bubbles,
    [Display(Name = "🛗")] Elevator,
    [Display(Name = "🪞")] Mirror,
    [Display(Name = "🪟")] Window,
    [Display(Name = "🪪")] IdentificationCard,
    [Display(Name = "⛔")] NoEntry,
    [Display(Name = "🚫")] Prohibited,
    [Display(Name = "📵")] NoMobilePhones,
    [Display(Name = "☢")] Radioactive,
    [Display(Name = "☣")] Biohazard,
    [Display(Name = "⬆")] UpArrow,
    [Display(Name = "➡")] RightArrow,
    [Display(Name = "⬇")] DownArrow,
    [Display(Name = "⬅")] LeftArrow,
    [Display(Name = "↕")] UpDownArrow,
    [Display(Name = "↔")] LeftRightArrow,
    [Display(Name = "↩")] RightArrowCurvingLeft,
    [Display(Name = "↪")] LeftArrowCurvingRight,
    [Display(Name = "⤴")] RightArrowCurvingUp,
    [Display(Name = "⤵")] RightArrowCurvingDown,
    [Display(Name = "🔃")] ClockwiseVerticalArrows,
    [Display(Name = "🔄")] CounterClockwiseArrowsButton,
    [Display(Name = "🔙")] BackArrow,
    [Display(Name = "🔚")] EndArrow,
    [Display(Name = "🔝")] TopArrow,
    [Display(Name = "🔁")] RepeatButton,
    [Display(Name = "🔂")] RepeatSingleButton,
    [Display(Name = "🔀")] ShuffleTracksButton,
    [Display(Name = "▶")] PlayButton,
    [Display(Name = "◀")] ReverseButton,
    [Display(Name = "⏩")] FastForwardButton,
    [Display(Name = "⏭")] NextTrackButton,
    [Display(Name = "⏪")] FastReverseButton,
    [Display(Name = "⏮")] LastTrackButton,
    [Display(Name = "🔼")] UpwardsButton,
    [Display(Name = "⏫")] FastUpButton,
    [Display(Name = "🔽")] DownwardsButton,
    [Display(Name = "⏬")] FastDownButton,
    [Display(Name = "📶")] AntennaBars,
    [Display(Name = "🛜")] Wireless,
    [Display(Name = "🟰")] HeavyEqualsSign,
    [Display(Name = "♾")] Infinity,
    [Display(Name = "🛏")] Bed,
    [Display(Name = "⚗")] Alembic,
    [Display(Name = "🔨")] Hammer,
    [Display(Name = "🪓")] Axe,
    [Display(Name = "⚒")] HammerAndPick,
    [Display(Name = "🛠")] HammerAndWrench,
    [Display(Name = "🗡")] Dagger,
    [Display(Name = "⚔")] CrossedSwords,
    [Display(Name = "💣")] Bomb,
    [Display(Name = "🏹")] BowAndArrow,
    [Display(Name = "🛡")] Shield,
    [Display(Name = "🪚")] CarpentrySaw,
    [Display(Name = "✉")] Envelope,
    [Display(Name = "📧")] Email,
    [Display(Name = "⏭")] Next,
    [Display(Name = "⏮")] Prev,
    [Display(Name = "✅")] Ok,
    [Display(Name = "❌")] Failed,
    [Display(Name = "❌")] CrossMark,
    [Display(Name = "↩")] Back,
    [Display(Name = "🆘")] SosButton,
    [Display(Name = "🚩")] TriangularFlag,
    [Display(Name = "🎌")] CrossedFlags,
    [Display(Name = "🏴")] BlackFlag,
    [Display(Name = "🏳")] WhiteFlag,
    [Display(Name = "🏴‍☠️")] PirateFlag,
    [Display(Name = "☠")] SkullAndCrossbones,
    [Display(Name = "💀")] Skull,
    [Display(Name = "🧠")] Brain,
    [Display(Name = "🫁")] Lungs,
    [Display(Name = "🦷")] Tooth,
    [Display(Name = "💨")] DashingAway,
    [Display(Name = "💦")] SweatDroplets,
    [Display(Name = "💢")] AngerSymbol,
    [Display(Name = "👁")] Eye,
    [Display(Name = "👀")] Eyes,
    [Display(Name = "🕵")] Detective,
    [Display(Name = "🥷")] Ninja,
    [Display(Name = "✍")] WritingHand,
    [Display(Name = "👳")] PersonWearingTurban,
    [Display(Name = "👲")] PersonWithSkullcap,
    [Display(Name = "🤵")] PersonInTuxedo,
    [Display(Name = "👨‍✈️")] PersonPilot,
    [Display(Name = "👨‍🎨️")] PersonArtist,
    [Display(Name = "👨‍💻")] PersonTechnologist,
    [Display(Name = "👩‍🔬")] PersonScienist,
    [Display(Name = "👩‍💼")] PersonOfficeWorker,
    [Display(Name = "👨‍🏭")] PersonFactoryWorker,
    [Display(Name = "👩‍🔧")] PersonMechanic,
    [Display(Name = "👨‍🍳")] PersonCook,
    [Display(Name = "👩‍⚖️")] PersonJudge,
    [Display(Name = "👩‍🏫")] PersonTeacher,
    [Display(Name = "👨‍🎓")] PersonManStudent,
    [Display(Name = "👩‍🎓")] PersonWomenStudent,
    [Display(Name = "👨‍⚕️")] PersonHealthWorker,
    [Display(Name = "👨‍⚕️")] PersonDoctor,
    [Display(Name = "👰")] PersonWithVeil,
    [Display(Name = "🧕")] PersonWithHeadscarf,
    [Display(Name = "🤴")] Prince,
    [Display(Name = "👸")] PersonPrincess,
    [Display(Name = "💂")] PersonGuard,
    [Display(Name = "👮")] PersonPoliceOfficer,
    [Display(Name = "👨‍🚒")] PersonFirefighter,
    [Display(Name = "👩‍🚀")] PersonAstronaut,
    [Display(Name = "👷")] PersonConstructionWorker,
    [Display(Name = "🧚")] Fairy,
    [Display(Name = "🎅")] SantaClaus,
    [Display(Name = "🧙")] Mage,
    [Display(Name = "🧛")] Vampire,
    [Display(Name = "🦹")] Supervillain,
    [Display(Name = "🦸")] Superhero,
    [Display(Name = "🧟")] Zombie,
    [Display(Name = "🧌")] Troll,
    [Display(Name = "🧞")] Genie,
    [Display(Name = "🚶")] PersonWalking,
    [Display(Name = "🧍")] PersonStanding,
    [Display(Name = "🏃")] PersonRunning,
    [Display(Name = "🧗")] PersonClimbing,
    [Display(Name = "🤺")] PersonFencing,
    [Display(Name = "🏇")] HorseRacing,
    [Display(Name = "⛷")] Skier,
    [Display(Name = "🚵")] PersonMountainBiking,
    [Display(Name = "🚴")] PersonBiking,
    [Display(Name = "🏋")] PersonLiftingWeights,
    [Display(Name = "⛹")] PersonBouncingBall,
    [Display(Name = "🏊")] PersonSwimming,
    [Display(Name = "🚣")] PersonRowingBoat,
    [Display(Name = "🏂")] Snowboarder,
    [Display(Name = "🏌")] PersonGolfing,
    [Display(Name = "🏄")] PersonSurfing,
    [Display(Name = "🤼")] PeopleWrestling,
    [Display(Name = "🤸")] PersonCartwheeling,
    [Display(Name = "🤽")] PersonPlayingWaterPolo,
    [Display(Name = "🤾")] PersonPlayingHandball,
    [Display(Name = "🧘")] PersonInLotusPosition,
    [Display(Name = "🗣")] SpeakingHead,
    [Display(Name = "🧜")] Merperson,
    [Display(Name = "👥")] BustsInSilhouette,
    [Display(Name = "👤")] BustInSilhouette,
    [Display(Name = "🐒")] Monkey,
    [Display(Name = "🦍")] Gorilla,
    [Display(Name = "🐕")] Dog,
    [Display(Name = "🐴")] HorseFace,
    [Display(Name = "🐎")] Horse,
    [Display(Name = "🦌")] Deer,
    [Display(Name = "🦏")] Rhinoceros,
    [Display(Name = "🦣")] Mammoth,
    [Display(Name = "👋")] Welcome,
    [Display(Name = "🛑")] Stop,
    [Display(Name = "✖")] No,
    [Display(Name = "✖")] Multiply,
    [Display(Name = "➕")] Plus,
    [Display(Name = "➖")] Minus,
    [Display(Name = "➗")] Divide,
    [Display(Name = "〰")] WavyDash,
    [Display(Name = "💱")] CurrencyExchange,
    [Display(Name = "💲")] HeavyDollarSign,
    [Display(Name = "⚕")] MedicalSymbol,
    [Display(Name = "🔱")] TridentEmblem,
    [Display(Name = "⭕")] HollowRedCircle,
    [Display(Name = "❓")] Question,
    [Display(Name = "❓")] RedQuestionMark,
    [Display(Name = "❔")] WhiteQuestionMark,
    [Display(Name = "❕")] WhiteExclamationMark,
    [Display(Name = "❗")] RedExclamationMark,
    [Display(Name = "♻")] RecyclingSymbol,
    [Display(Name = "📛")] NameBadge,
    [Display(Name = "⚜")] FleurDeLis,
    [Display(Name = "🔰")] JapaneseSymbolForBeginner,
    [Display(Name = "☑")] CheckBoxWithCheck,
    [Display(Name = "🔠")] InputLatinUppercase,
    [Display(Name = "👹")] Ogre,
    [Display(Name = "👺")] Goblin,
    [Display(Name = "👻")] Ghost,
    [Display(Name = "👽")] Alien,
    [Display(Name = "✔")] Yes,
    [Display(Name = "❎")] CrossMarkButton,
    [Display(Name = "➰")] CurlyLoop,
    [Display(Name = "➿")] DoubleCurlyLoop,
    [Display(Name = "〽")] PartAlternationMark,
    [Display(Name = "✳")] EightSpokedAsterisk,
    [Display(Name = "✴")] EightPointedStar,
    [Display(Name = "❇")] Sparkle,
    [Display(Name = "™")] TradeMark,
    [Display(Name = "®")] Registered,
    [Display(Name = "©")] Copyright,
    [Display(Name = "#️⃣")] KeycapSharp,
    [Display(Name = "*️⃣")] KeycapStar,
    [Display(Name = "0️⃣")] Keycap0,
    [Display(Name = "1️⃣")] Keycap1,
    [Display(Name = "2️⃣")] Keycap2,
    [Display(Name = "3️⃣")] Keycap3,
    [Display(Name = "4️⃣")] Keycap4,
    [Display(Name = "5️⃣")] Keycap5,
    [Display(Name = "6️⃣")] Keycap6,
    [Display(Name = "7️⃣")] Keycap7,
    [Display(Name = "8️⃣")] Keycap8,
    [Display(Name = "9️⃣")] Keycap9,
    [Display(Name = "🔣")] InputSymbols,
    [Display(Name = "🔢")] InputNumbers,
    [Display(Name = "🔡")] InputLatinLowercase,
    [Display(Name = "🔤")] InputLatinLetters,
    [Display(Name = "ℹ")] Information,
    [Display(Name = "🆕")] NewButton,
    [Display(Name = "🆔")] IdButton,
    [Display(Name = "🆗")] OkButton,
    [Display(Name = "🔜")] SoonArrow,
    [Display(Name = "🔛")] OnArrow,
    [Display(Name = "🆓")] FreeButton,
    [Display(Name = "🅰")] BloodTypeButtonA,
    [Display(Name = "🆎")] BloodTypeButtonAb,
    [Display(Name = "🅱")] BloodTypeButtonB,
    [Display(Name = "🆑")] BloodTypeButtonCl,
    [Display(Name = "🈁")] JapaneseHereButton,
    [Display(Name = "🈂")] JapaneseServiceChargeButton,
    [Display(Name = "🈷")] JapaneseMonthlyAmountButton,
    [Display(Name = "🈶")] JapaneseNotFreeOfChargeButton,
    [Display(Name = "🈯")] JapaneseReservedButton,
    [Display(Name = "🉐")] JapaneseBargainButton,
    [Display(Name = "🈹")] JapaneseDiscountButton,
    [Display(Name = "🈚")] JapaneseFreeOfChargeButton,
    [Display(Name = "🈲")] JapaneseProhibitedButton,
    [Display(Name = "🉑")] JapaneseAcceptableButton,
    [Display(Name = "🈸")] JapaneseApplicationButton,
    [Display(Name = "🈴")] JapanesePassingGradeButton,
    [Display(Name = "🈳")] JapaneseVacancyButton,
    [Display(Name = "㊗")] JapaneseCongratulationsButton,
    [Display(Name = "㊙")] JapaneseSecretButton,
    [Display(Name = "🈺")] JapaneseOpenForBusinessButton,
    [Display(Name = "🈵")] JapaneseNoVacancyButton,
    [Display(Name = "🅿")] Parkingbutton,
    [Display(Name = "🔺")] RedTrianglePointedUp,
    [Display(Name = "🔻")] RedTrianglePointedDown,
    [Display(Name = "🔘")] RadioButton,
    [Display(Name = "✔")] CheckMark,
    [Display(Name = "🖥")] DesktopComputer,
    [Display(Name = "🕯")] Candle,
    [Display(Name = "🔦")] Flashlight, // research
    [Display(Name = "🎬")] ClapperBoard,
    [Display(Name = "⏳")] HourGlass,
    [Display(Name = "⚙")] Gear,
    [Display(Name = "🔧")] Wrench,
    [Display(Name = "🪛")] Screwdriver,
    [Display(Name = "🔩")] NutAndBolt,
    [Display(Name = "🌎")] Globe,
    [Display(Name = "🌐")] GlobeWithMeridians,
    [Display(Name = "📢")] LoudSpeaker,
    [Display(Name = "📣")] Megaphone,
    [Display(Name = "🔇")] MutedSpeaker,
    [Display(Name = "🔥")] Fire,
    [Display(Name = "⚠")] Warning,
    [Display(Name = "ℹ")] Info,
    [Display(Name = "🔒")] Locked,
    [Display(Name = "🔓")] Unlocked,
    [Display(Name = "🔏")] LockedWithPen,
    [Display(Name = "🔐")] LockedWithKey,
    [Display(Name = "🔑")] Key,
    [Display(Name = "🗑")] WasteBasket,
    [Display(Name = "📌")] PushPin,
    [Display(Name = "📍")] RoundPushpin,
    [Display(Name = "🗜")] Clamp,
    [Display(Name = "⛏")] Pick,
    [Display(Name = "📎")] Paperclip,
    [Display(Name = "📅")] Calendar,
    [Display(Name = "📆")] TearOffCalendar,
    [Display(Name = "🗓")] SpiralCalendar,
    [Display(Name = "✏")] Pencil,
    [Display(Name = "🖊")] Pen,
    [Display(Name = "🖋")] FountainPen,
    [Display(Name = "✒")] BlackNib,
    [Display(Name = "🖌")] Paintbrush,
    [Display(Name = "📝")] Memo,
    [Display(Name = "🖍")] Crayon,
    [Display(Name = "📬")] Mailbox,
    [Display(Name = "🧫")] PetriDish,
    [Display(Name = "🧬")] Dna,
    [Display(Name = "💉")] Syringe,
    [Display(Name = "🩸")] DropOfBlood,
    [Display(Name = "🗿")] Moai,
    [Display(Name = "🧿")] NazarAmulet,
    [Display(Name = "⚱")] FuneralUrn,
    [Display(Name = "🧺")] Basket,
    [Display(Name = "🪦")] Headstone,
    [Display(Name = "⚰")] Coffin,
    [Display(Name = "🚬")] Cigarette,
    [Display(Name = "🪥")] Toothbrush,
    [Display(Name = "🧽")] Sponge,
    [Display(Name = "🏧")] AtmSign,
    [Display(Name = "🚾")] WaterCloset,
    [Display(Name = "🛂")] PassportControl,
    [Display(Name = "🛃")] Customs,
    [Display(Name = "🛄")] BaggageClaim,
    [Display(Name = "🛅")] LeftLuggage,
    [Display(Name = "🚸")] ChildrenCrossing,
    [Display(Name = "🔞")] NoOneUnderEighteen,
    [Display(Name = "⏸")] PauseButton,
    [Display(Name = "⏹")] StopButton,
    [Display(Name = "⏺")] RecordButton,
    [Display(Name = "⏏")] EjectButton,
    [Display(Name = "🔅")] DimButton,
    [Display(Name = "🔆")] BrightButton,

    [Display(Name = "🔌")] ElectricPlug,
    [Display(Name = "🪫")] LowBattery,
    [Display(Name = "🔋")] Battery,
    [Display(Name = "🕹")] Joystick,
    [Display(Name = "🎰")] SlotMachine,
    [Display(Name = "♠")] SpadeSuit,
    [Display(Name = "♥")] HeartSuit,
    [Display(Name = "♦")] DiamondSuit,
    [Display(Name = "♣")] ClubSuit,
    [Display(Name = "🀄")] MahjongRedDragon,
    [Display(Name = "🎴")] FlowerPlayingCards,
    [Display(Name = "🎮")] VideoGame,
    [Display(Name = "☂")] Umbrella,
    [Display(Name = "🌡")] Thermometer,
    [Display(Name = "⏱")] Stopwatch,
    [Display(Name = "🙏")] Please,
    [Display(Name = "💤")] Zzz,
    [Display(Name = "😺")] GrinningCat,
    [Display(Name = "😸")] GrinningCatWithSmilingEyes,
    [Display(Name = "😹")] CatWithTearsOfJoy,
    [Display(Name = "😻")] SmilingCatWithHeartEyes,
    [Display(Name = "😼")] CatWithWrySmile,
    [Display(Name = "😽")] KissingCat,
    [Display(Name = "🙀")] WearyCat,
    [Display(Name = "😿")] CryingCat,
    [Display(Name = "😾")] PoutingCat,
    [Display(Name = "👾")] AlienMonster,
    [Display(Name = "🤖")] Robot,
    [Display(Name = "🗃️")] CardFileBox,
    [Display(Name = "⌨")] Keyboard,
    [Display(Name = "🖱")] ComputerMouse,
    [Display(Name = "💗")] GrowingHeart,
    [Display(Name = "💯")] HundredPoints,
    [Display(Name = "💥")] Collision,
    [Display(Name = "💫")] Dizzy,
    [Display(Name = "🕳")] Hole,
    [Display(Name = "💬")] SpeechBalloon,
    [Display(Name = "🗨")] LeftSpeechBubble,
    [Display(Name = "🦾")] MechanicalArm,
    [Display(Name = "🦴")] Bone,
    [Display(Name = "💻")] Laptop,
    [Display(Name = "📷")] Camera,
    [Display(Name = "🔍")] MagnifyingGlass,
    [Display(Name = "🔎")] MagnifyingGlassTiltedRight,
    [Display(Name = "🔍")] MagnifyingGlassTiltedLeft,
    [Display(Name = "💡")] LightBulb,
    [Display(Name = "🏮")] RedPaperLantern,
    [Display(Name = "🪔")] DiyaLamp,
    [Display(Name = "📖")] OpenBook,
    [Display(Name = "📗")] GreenBook,
    [Display(Name = "📚")] Books,
    [Display(Name = "📓")] Notebook,
    [Display(Name = "📒")] Ledger,
    [Display(Name = "📃")] PageWithCurl,
    [Display(Name = "🗞")] RolledUpNewspaper,
    [Display(Name = "📑")] BookmarkTabs,
    [Display(Name = "🔖")] Bookmark,
    [Display(Name = "💸")] MoneyWithWings,
    [Display(Name = "💳")] CreditCard,
    [Display(Name = "💹")] ChartIncreasingWithYen,
    [Display(Name = "📨")] IncomingEnvelope,
    [Display(Name = "📩")] EnvelopeWithArrow,
    [Display(Name = "🗒")] SpiralNotepad,
    [Display(Name = "🖇")] LinkedPaperclips,
    [Display(Name = "🪙")] Coin,
    [Display(Name = "📙")] OrangeBook,
    [Display(Name = "📜")] Scroll,
    [Display(Name = "📄")] PageFacingUp,
    [Display(Name = "📰")] Newspaper,
    [Display(Name = "🏷")] Label,
    [Display(Name = "💰")] MoneyBag,
    [Display(Name = "🧾")] Receipt,
    [Display(Name = "📤")] OutboxTray,
    [Display(Name = "📥")] InboxTray,
    [Display(Name = "📦")] Package,
    [Display(Name = "📮")] Postbox,
    [Display(Name = "🗳")] BallotBoxWithBallot,
    [Display(Name = "📘")] BlueBook,
    [Display(Name = "💵")] Dollar,
    [Display(Name = "📈")] Chart,
    [Display(Name = "📈")] ChartIncreasing,
    [Display(Name = "📉")] ChartDecreasing,
    [Display(Name = "📊")] BarChart,
    [Display(Name = "📋")] Clipboard,
    [Display(Name = "📏")] StraightRuler,
    [Display(Name = "📐")] TriangularRuler,
    [Display(Name = "✂")] Scissors,
    [Display(Name = "💊")] Pill,
    [Display(Name = "🧯")] FireExtinguisher,
    [Display(Name = "🛒")] ShoppingCart,
    [Display(Name = "🪧")] Placard,
    [Display(Name = "🗎")] Document,
    [Display(Name = "🎵")] MusicalNote,
    [Display(Name = "🎶")] MusicalNotes,
    [Display(Name = "🎤")] Microphone,
    [Display(Name = "🎧")] Headphone,
    [Display(Name = "🎸")] Guitar,
    [Display(Name = "🎷")] Saxophone,
    [Display(Name = "🪗")] Accordion,
    [Display(Name = "🎹")] MusicalKeyboard,
    [Display(Name = "🎺")] Trumpet,
    [Display(Name = "🎻")] Violin,
    [Display(Name = "📟")] Pager,
    [Display(Name = "📠")] FaxMachine,
    [Display(Name = "☎")] Telephone,
    [Display(Name = "🖨")] Printer,
    [Display(Name = "💽")] ComputerDisk,
    [Display(Name = "📀")] Dvd,
    [Display(Name = "💿")] OpticalDisk,
    [Display(Name = "💾")] FloppyDisk,
    [Display(Name = "🖲")] Trackball,
    [Display(Name = "🧮")] Abacus,
    [Display(Name = "🎥")] MovieCamera,
    [Display(Name = "🎞")] FilmFrames,
    [Display(Name = "📽")] FilmProjector,
    [Display(Name = "📺")] Television,
    [Display(Name = "📸")] CameraWithFlash,
    [Display(Name = "📹")] VideoCamera,
    [Display(Name = "📻")] Radio,
    [Display(Name = "🥁")] Drum,
    [Display(Name = "📱")] MobilePhone,
    [Display(Name = "📲")] MobilePhoneWithArrow,
    [Display(Name = "📞")] TelephoneReceiver,
    [Display(Name = "🖼️")] Picture,
    [Display(Name = "🖼️")] FramedPicture,
    [Display(Name = "🎨")] ArtistPalette,
    [Display(Name = "🧵")] Thread,
    [Display(Name = "🧶")] Yarn,
    [Display(Name = "🪢")] Knot,
    [Display(Name = "🪡")] SewingNeedle,
    [Display(Name = "👓")] Glasses,
    [Display(Name = "🕶")] Sunglasses,
    [Display(Name = "👜")] Bandbag,
    [Display(Name = "🛍")] ShoppingBags,
    [Display(Name = "🩴")] ThongSandal,
    [Display(Name = "🎓")] GraduationCap,
    [Display(Name = "👑")] Crown,
    [Display(Name = "📯")] PostalHorn,
    [Display(Name = "🔔")] Bell,
    [Display(Name = "🔕")] BellWithSlash,
    [Display(Name = "📹")] Video,
    [Display(Name = "🔹")] SmallBlueDiamond,
    [Display(Name = "🔸")] SmallOrangeDiamond,
    [Display(Name = "🔷")] LargeBlueDiamond,
    [Display(Name = "🔶")] LargeOrangeDiamond,
    [Display(Name = "▫")] WhiteSmallSquare,
    [Display(Name = "▪")] BlackSmallSquare,
    [Display(Name = "◽")] WhiteMediumSmallSquare,
    [Display(Name = "◾")] BlackMediumSmallSquare,
    [Display(Name = "◻")] WhiteMediumSquare,
    [Display(Name = "◼")] BlackMediumSquare,
    [Display(Name = "⬜")] WhiteLargeSquare,
    [Display(Name = "⬛")] BlackLargeSquare,
    [Display(Name = "🟫")] BrownSquare,
    [Display(Name = "🟪")] PurpleSquare,
    [Display(Name = "🟦")] BlueSquare,
    [Display(Name = "🟩")] GreenSquare,
    [Display(Name = "🟨")] YellowSquare,
    [Display(Name = "🟧")] OrangeSquare,
    [Display(Name = "🟥")] RedSquare,
    [Display(Name = "⚪")] WhiteCircle,
    [Display(Name = "🟤")] BrownCircle,
    [Display(Name = "🟣")] PurpleCircle,
    [Display(Name = "⚫")] BlackCircle,
    [Display(Name = "🔵")] BlueCircle,
    [Display(Name = "🟢")] GreenCircle,
    [Display(Name = "🟡")] YellowCircle,
    [Display(Name = "🟠")] OrangeCircle,
    [Display(Name = "🔴")] RedCircle,
    [Display(Name = "📕")] ClosedBook,
    [Display(Name = "🚧")] Construction,
    [Display(Name = "🛟")] RingBuoy,
    [Display(Name = "🚀")] Rocket,
    [Display(Name = "🛎")] BellhopBell,
    [Display(Name = "⏰")] AlarmClock,
    [Display(Name = "🌙")] CrescentMoon,
    [Display(Name = "☀")] Sun,
    [Display(Name = "🪐")] Saturn,
    [Display(Name = "⭐")] Star,
    [Display(Name = "⛅")] SunBehindCloud,
    [Display(Name = "🌧")] CloudWitRain,
    [Display(Name = "⛈")] CloudWithLightningAndRain,
    [Display(Name = "🌤")] SunBehindSmallCloud,
    [Display(Name = "🌥")] SunBehindLargeCloud,
    [Display(Name = "🌦")] SunBehindRainCloud,
    [Display(Name = "🌨")] CloudWithSnow,
    [Display(Name = "🌫")] Fog,
    [Display(Name = "🌬")] WindFace,
    [Display(Name = "🌈")] Rainbow,
    [Display(Name = "🌂")] ClosedUmbrella,
    [Display(Name = "☔")] UmbrellaWithRainDrops,
    [Display(Name = "☃")] Snowman,
    [Display(Name = "⛄")] SnowmanWithoutSnow,
    [Display(Name = "🎃")] JackOLantern,
    [Display(Name = "🎄")] ChristmasTree,
    [Display(Name = "🎆")] Fireworks,
    [Display(Name = "🎇")] Sparkler,
    [Display(Name = "✨")] Sparkles,
    [Display(Name = "🎊")] ConfettiBall,
    [Display(Name = "🎀")] Ribbon,
    [Display(Name = "🎗")] ReminderRibbon,
    [Display(Name = "🥋")] MartialArtsUniform,
    [Display(Name = "⛳")] FlagInHole,
    [Display(Name = "🎣")] FishingPole,
    [Display(Name = "🤿")] DivingMask,
    [Display(Name = "🎿")] Skis,
    [Display(Name = "🛷")] Sled,
    [Display(Name = "🪀")] YoYo,
    [Display(Name = "🪁")] Kite,
    [Display(Name = "🔮")] CrystalBall,
    [Display(Name = "🧩")] PuzzlePiece,
    [Display(Name = "🧸")] TeddyBear,
    [Display(Name = "♟")] ChessPawn,
    [Display(Name = "🃏")] Joker,
    [Display(Name = "🎭")] PerformingArts,
    [Display(Name = "🥼")] LabCoat,
    [Display(Name = "🦺")] SafetyVest,
    [Display(Name = "👔")] Necktie,
    [Display(Name = "👖")] Jeans,
    [Display(Name = "🧣")] Scarf,
    [Display(Name = "🧥")] Coat,
    [Display(Name = "👕")] TiShirt,
    [Display(Name = "🧦")] Socks,
    [Display(Name = "👗")] Dress,
    [Display(Name = "🩲")] Briefs,
    [Display(Name = "🩳")] Shorts,
    [Display(Name = "👙")] Bikini,
    [Display(Name = "👘")] Kimono,
    [Display(Name = "🎒")] Backpack,
    [Display(Name = "👟")] RunningShoe,
    [Display(Name = "🥾")] HikingBoot,
    [Display(Name = "🎩")] TopHat,
    [Display(Name = "🧢")] BilledCap,
    [Display(Name = "🪖")] MilitaryHelmet,
    [Display(Name = "⛑")] RescueWorkersHelmet,
    [Display(Name = "👒")] WomansHat,
    [Display(Name = "📿")] PrayerBeads,
    [Display(Name = "💍")] Ring,
    [Display(Name = "💎")] GemStone,
    [Display(Name = "🔈")] SpeakerLowVolume,
    [Display(Name = "🔉")] SpeakerMediumVolume,
    [Display(Name = "🔊")] SpeakerHighVolume,
    [Display(Name = "🎼")] MusicalScore,
    [Display(Name = "🎙")] StudioMicrophone,
    [Display(Name = "🎚")] LevelSlider,
    [Display(Name = "🎛")] ControlKnobs,
    [Display(Name = "🌪")] Tornado,
    [Display(Name = "⚡")] HighVoltage,
    [Display(Name = "❄")] Snowflake,
    [Display(Name = "☄")] Comet,
    [Display(Name = "💧")] Droplet,
    [Display(Name = "🌊")] WaterWave,
    [Display(Name = "🧨")] Firecracker,
    [Display(Name = "🎈")] Balloon,
    [Display(Name = "🎉")] PartyPopper,
    [Display(Name = "🎁")] WrappedGift,
    [Display(Name = "🎫")] Ticket,
    [Display(Name = "🎟")] AdmissionTickets,
    [Display(Name = "🎖")] MilitaryMedal,
    [Display(Name = "🏆")] Trophy,
    [Display(Name = "🏅")] SportMedal,
    [Display(Name = "🥇")] FirstPlaceMedal,
    [Display(Name = "🥈")] SecondPlaceMedal,
    [Display(Name = "🥉")] ThirdPlaceMedal,
    [Display(Name = "🥇")] GoldMedal,
    [Display(Name = "🥈")] SilverMedal,
    [Display(Name = "🥉")] BronzeMedal,
    [Display(Name = "🥊")] BoxingGlove,
    [Display(Name = "🎯")] Bullseye,
    [Display(Name = "🔫")] WaterPistol,
    [Display(Name = "🪄")] MagicWand,
    [Display(Name = "🎲")] GameDice,
    [Display(Name = "🪩")] MirrorBall,
    [Display(Name = "🏁")] FinishFlag,
    [Display(Name = "🇦🇿")] Azerbaijan,
}