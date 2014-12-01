using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using SharpEntropy;
using SharpEntropy.IO;

namespace Test
{
    class Program
    {
        private static readonly string currentDirectory = Environment.CurrentDirectory + "/../../";

        static void Main(string[] args)
        {
            /*// read file
            var tokenizerTrainingFilePath = currentDirectory + "Input/tokenizer.train";
            var outputFilePath = currentDirectory + "Output/EnglishTok.nbin";
            MaximumEntropyTokenizer.Train(tokenizerTrainingFilePath, outputFilePath);*/
            
            // test detokenization
            /*var tokens = new List<string>() {"do", "n't", "commit"};
            var detokenizer = new DictionaryDetokenizer();
            var result = detokenizer.Detokenize(tokens.ToArray());
            Console.WriteLine(result);*/

            /*// train model file
            var inputFilePath = currentDirectory + "Input/sentences.train";
            var outputFilePath = currentDirectory + "Output/" + Path.GetFileNameWithoutExtension(inputFilePath) + ".nbin";
            var iterations = 100;
            var cut = 5;
            var endOfSentenceScanner = new CharactersSpecificEndOfSentenceScanner();
            Console.WriteLine("Training model...");
            var model = MaximumEntropySentenceDetector.TrainModel(inputFilePath, iterations, cut, endOfSentenceScanner);
            Console.WriteLine("Writing output file '{0}'...", outputFilePath);
            new BinaryGisModelWriter().Persist(model, outputFilePath);
            Console.WriteLine("Output file written.");*/

            /*// tokenize tests
            var modelPath = currentDirectory + "../Resources/Models/";
            var tokenizer = new EnglishMaximumEntropyTokenizer(modelPath + "EnglishTok.nbin");

            var input = "It was built of a bright brick throughout; its skyline was fantastic, and even its ground plan was wild.";
            var tokens = tokenizer.Tokenize(input);
            Console.WriteLine(string.Join(" | ", tokens));*/

            // invalid email detection
            var inputFilePath = currentDirectory + "Input/invalidEmailDetection.train";
            var outputFilePath = currentDirectory + "Output/" + Path.GetFileNameWithoutExtension(inputFilePath) + ".nbin";
            var iterations = 100;
            var cut = 5;
            Console.WriteLine("Training model...");
            var model = MaximumEntropyInvalidEmailDetector.TrainModel(inputFilePath, iterations, cut);
            Console.WriteLine("Writing output file '{0}'...", outputFilePath);
            new BinaryGisModelWriter().Persist(model, outputFilePath);
            Console.WriteLine("Output file written.");

            // test trained model
            var results = new List<Tuple<string, bool, double>>();
            var invalidEmailDetector = new MaximumEntropyInvalidEmailDetector(outputFilePath);
            var allLines = File.ReadAllLines(inputFilePath);
            foreach (var line in allLines)
            {
                var parts = line.Split('\t');
                var email = parts.First();
                var isInvalid = parts.Last() == "1";
                
                var invalidProbability = invalidEmailDetector.Detect(email);
                //var moreLikelyThanAverageToBeInvalid = invalidProbability*100 > 24.97;
                results.Add(new Tuple<string, bool, double>(email, isInvalid, invalidProbability));
            }

            var nbOfEmailsSent = 0;
            var nbOfEmailsSentWhichWouldBounce = 0;
            foreach (var result in results.OrderBy(tup => tup.Item3))
            {
                if (nbOfEmailsSentWhichWouldBounce < 25)
                {
                    nbOfEmailsSent++;
                    if (result.Item2) { nbOfEmailsSentWhichWouldBounce++; }
                }
                Console.WriteLine("{0} ({1})", result.Item1, result.Item2 ? "INVALID": "OK");
            }

            Console.WriteLine("Email that could have been sent: {0}", nbOfEmailsSent);

            Console.WriteLine("========");

            var probaOfInvalidity = 0.2497;
            var nbOfSamples = results.Count;
            Console.WriteLine("Nb of samples: {0}", nbOfSamples);
            var nbOfCorrectResults = results.Count(tup => tup.Item2 == tup.Item3 > probaOfInvalidity);
            Console.WriteLine("Nb of correct results: {0}", nbOfCorrectResults);
            var nbOfNotDetected = results.Count(tup => tup.Item2 && !(tup.Item3 > probaOfInvalidity));
            Console.WriteLine("Nb of not detected: {0}", nbOfNotDetected);
            var nbOfFalsePositive = results.Count(tup => !tup.Item2 && (tup.Item3 > probaOfInvalidity));
            Console.WriteLine("Nb of false positive: {0}", nbOfFalsePositive);

            Console.WriteLine("===========");

            var newUsersToProbabiltyOfInvalidity = TestRegisteredUsers
                .Select(u => new Tuple<string, double>(u, invalidEmailDetector.Detect(u)))
                .OrderBy(tup => tup.Item2)
                .ToList();
            foreach (var tuple in newUsersToProbabiltyOfInvalidity)
            {
                Console.WriteLine("{0} - {2} ({1})", tuple.Item1, tuple.Item2, tuple.Item1.Split('@').First().ToCharArray().Distinct().Count());
            }


            Console.WriteLine("OK");
            Console.ReadKey();
        }
        private static List<string> TestRegisteredUsers = new List<string>() { "fhftfytuf@hotmail.com", "sahin.sen@gmail.com", "mouhou_1@hotmail.com", "faku_garbaccio@hotmail.com", "neves@gmail.com", "thiagomagnago@hotmail.com", "kaisergruber.corentin@hotmail.com", "detona_mundo2013@hotmail.com", "icee_creaam@hotmail.fr", "peitiola@gmail.com", "sdfasd@c.com", "aerorios@gmail.com", "jonatasbm@gmail.com", "helene75014@gmail.com", "as@yahoo.com", "ninguna77@live.fr", "kevin-lutonadio@hotmail.fr", "lol@hotmail.fr", "sergioinuma@outlook.com.br", "lolipop@hotmail.fr", "magnoguedesotavio@gmail.com", "nestormoreno14@gmail.com", "kop@hotmail.fr", "mp@hotmail.fr", "ka_linux@yahoo.com", "ibsaal90@hotmail.com", "ninguna91@live.fr", "mg.real18@gmail.com", "rodriguez_victor@hotmail.com", "aninhachia@hotmail.com", "ar7li_@hotmail.com", "guiosantosferreira@hotmail.com", "hgllsjf@hotmail.com", "adeguve@hotmail.com", "hrty@mail.com", "mahmoudeldiasty38@yahoo.com", "kllkllk@ioioijoj.com", "paulo.citino@adventistas.org.br", "iijijihyfd@huhih.com", "ar7li-@hotmail.com", "Jhonathane26@gmail.com", "1635464368@qq.com", "thiagolovejazz@gmail.com", "fernando_amx@hotmail.com", "asdasd@asdad.com", "ronaldsouzza@gmail.com", "aninhakia@hotmail.com", "totaanaanatota11@gmail.com", "jhggu@mail.com", "charles_del_Castillo@hotmail.es", "akashi.okdk@gmail.com", "Rasha11@hotmail.com", "daldal@erfs.com", "hglkhghkg@outlook.com", "zz@hh.xom", "yayaeeeeyaa@gmail.com", "imanis2@gmail.com", "mbkanabi@gmail.com", "sanazbndryn@yahoo.com", "askuki@free.fr", "1729745514@qq.com", "kadirkarandere@hotmail.com", "hfgdtegd@gmail.com", "cdvxfvgdf@gmail.com", "cdscfsdfcs@gmail.com", "gregtgtgtgtgtgte@gmail.com", "sherin2001@mail.ru", "pa.calonne@gmail.com", "z00613z97@gmail.com", "ski-shop@wanadoo.fr", "Langedu32@icloud.com", "jgalort@gmail.com", "rizzomelanie@hotmail.com", "eroulot@free.fr", "adel.bouzid@hotmail.fr", "laure_alexandra_g@hotmail.com", "saly.keita@aviva.com", "simply.shining@hotmail.com", "robertoperezin87@gmail.com", "villalba1989mlg@gmail.com", "ilopfe@gmail.com", "magoespa@gmail.com", "abdelftahyehia@yahoo.com", "antonioruizferrera@gmail.com", "frade1_@hotmail.com", "titinou49@gmail.com", "wernef@gmail.com", "jmp.jorge.jmp@gmail.com", "sporchet@hotmail.fr", "nasser1@gmail.com", "Victormillanzaragoza@gmail.com", "vincent1102@hotmail.fr", "jalejobcn@gmail.com", "brunetmathi@gmail.com", "vifrancia@hotmail.com", "adrian.maciaslopez@gmail.com", "xabifesta@gmail.com", "brunet.mathi@gmail.com", "verdiblanko11@gmail.com", "gabi87surfer@gmail.com", "elena.bad@gmail.com", "electronico.i42@gmail.com", "gio.ays@gmail.com", "swol1995@gmail.com", "eyrelmente@gmail.com", "mnt1986@gmail.com", "carlosregidorse@gmail.com", "danieldeba@yahoo.fr", "caraballo593@gmail.com", "elmikeli@hotmail.com", "nicolas.fernandez61@gmail.com", "dani3lin@gmail.com", "aog_75@hotmail.com", "jacentenohdez@gmail.com", "Klingelhofer@telia.com", "enrique.f1.89@gmail.com", "x_bloon@hotmail.com", "akosakuchillo@gmail.com", "bibillo05@gmail.com", "davidmg90@outlook.es", "antonio_jose_teru@hotmail.com", "sokhalmemaybe@gmail.com", "xaikina@hotmail.com", "gutiserra@gmail.com", "lamine@hotmail.com", "c389445@trbvm.com", "anneroncalli@gmail.com", "Cristiano.corr@hotmail.com", "lamine@hotmail.fr", "carlos_sango@hotmail.com", "sifros82@orange.fr", "Vlad.mazurenko@hotmail.es", "javierlomadominguez@gmail.com", "prmoh.udk999@gmail.com", "gtrgrgre@gmail.com", "ergegegee@gmail.com", "martacantero27@gmail.com", "julinhoec@hotmail.com", "javier.gonzalo.90@gmail.com", "jpiufvvh@live.fr", "bernardcelia@hotmail.fr", "mandou.stephane@orange.fr", "dfgdfrrrf@yahoo.com", "yguyguyguyg@hotmail.com", "jonathan.malka@me.com", "farkas.gabor.88@gmail.com", "rivero44@hotmail.com", "axel.delahaye@gmail.com", "iraburu12@hotmail.com", "antoine69740@live.fr", "alainsaintlucien@gmail.com", "adham.alhitar@gmail.com", "pedroluisjt@hotmail.com", "gereksizmailler98@yandex.com", "gfhghdfhgf@dgdfg.hdfg", "gladitor2080@hotmail.com", "vargos@gmx.com", "hjjhjkjdfg@hotmail.fr", "sbai3-1999@live.fr", "jamal_elmrini@msn.com", "r.fariasilva@gmail.com", "qusekikoo@outlook.fr", "diethrik@gmail.com", "joaohenrique15@hotmail.com", "toqueyann@gmail.com", "fghn@hotmail.fr", "sean_massandy@hotmail.fr", "nelsontrindade@gmail.com", "miguelrezende7@gmail.com", "qwertyuiop@hotmail.com", "QSXQcb@free.fr", "anneetnorbert@gmail.com", "dimitrisminotakis@yahoo.it", "kljaslkdaskh@gmail.com", "wxcxcvwxc@free.Fr", "fgjhvhgcjvyjhhbv@orange.fr", "pianijacopo@libero.it", "hammoudinardjess@gmail.com", "romainlambert69740@laposte.net", "lm@uol.com", "lm2@uol.com", "anais.vivien@gmail.com", "manonrozoy@orange.fr", "jon@gmail.com", "briceje@yahoo.com", "ikram123234@outlook.sa", "piripirimk@gmail.com", "romain.joly27@free.fr", "marjanesoufiane@gmail.com", "rgdrggr@yahoo.fr", "zxcdfrtyhgf@hotmail.com", "bea2001oliveira@outlook.pt", "f.beurrier@free.fr", "s.c.bezemer@gmail.com", "aureodiogo2014@gmail.com", "stnibedy@gmail.com", "vanhede.stephanie@gmail.com", "pascalregimbaud@gmail.com", "manpoma@hotmail.com", "oiugr@gege.com", "Aryclenes5001@gmail.com", "dias.gerson@live.fr", "juanvillaurquiza2014@gmail.com", "aminezaghlami@yahoo.fr", "naniemaza@live.fr", "haccpmaroc@gmail.com", "pppppppppp@hotmail.com", "gab7@yopmail.com", "davidov94@mail.ru", "sahdausd@yahoo.com.br", "hfzain@gmail.com", "chelomarc3@hotmail.com", "daniel.stormo@gmail.com", "m.abuelyouser@gmail.com", "jsgalvez94@hotmail.com", "alber4maldon@gmail.com", "joseluismassa@gmail.com", "hamdoune33.karim@gmail.com", "matheo2lazer@hotmail.fr", "jnebfenjfnjf@live.fr", "fbisks@live.fr", "dr@gmail.com", "Princeamir00@yahoo.com", "qedaedff@iwueguyg.com", "dani_slk@hotmail.com", "debrigode.adeline@gmail.com", "degottex@hotmail.com", "night_-_angle@hotmail.com", "sergiconor@gmail.com", "fggfffddf@gmail.com", "toufik.2903@hotmail.fr", "eloise.gaudet@lille.isefac.fr", "cdsvfsd@gmail.com", "marie_leon@hotmail.fr", "paleksandra940@gmail.com", "esther.gil.hidalgo@gmail.com", "koko.hanouka@yahoo.fr", "djpcutta@gmail.com", "wefcwdv@wdsfscrd.com", "jbsoftbr@yahoo.com.br", "ghfhvf@gmail.com", "Gaudet-Eloise@hotmail.fr", "sandy.verbeken@gmail.com", "apouleau@hotmail.fr", "andrivon.william@gmail.com", "tgerfqs@hotmail.fr", "franzao@live.com", "jofera@gmail.com", "ni.ko.xx@hotmail.com", "sofrueh@arcor.de", "d-diesel@live.fr", "a_fghje_f2@gmail.com", "magic86@live.com", "uhehuehu@yahoo.fr", "jghgjgj@gmail.com", "franfoi43@hotmail.fr", "timo.armanii@live.nl", "assia_bella5@hotmail.com", "gyllianb@gmail.com", "luiisaamado@gmail.com", "msn_nv8@hotmail.com", "serchie@outlook.com", "muslim563@yahoo.com", "vova388@mail.com", "mbilalturan@gmail.com", "tpe2014du38@gmail.com", "ijijijijijii@hotmail.com", "golfmora@hotmail.com", "antoniamazzeo@gmail.com", "yghjnftfyghhjn@56y7uyu.com", "vova388@gmail.com", "greenfeeb@hotmail.com", "edmilsonnse@gmail.com", "anthonyti@outlook.fr", "jbssanch@gmail.com", "deejaybiz@gmail.com", "morobra@yahoo.com.br", "romeo.29@hotmail.fr", "rdcneto@outlook.com", "djklwjfdk@jksldfj.jskldf", "dagrs70@gmail.com", "amoreira79@gmail.com", "essaiessai@orange.fr", "karinegrande@me.com", "hana.andom@gmail.com", "brice.duret@hotmail.fr", "grabinsky.s@gmail.com", "eoline@laposte.net", "isaacbokongo@gmail.com", "akminohara@gmail.com", "tjyyfjssjry@gmail.com", "julien.hassid@gmail.com", "svirgiliba@gmail.com", "salam2294@ymail.coml", "bunnyho0@forum.dk", "bunnyho0@ofir.dk", "jjdhdbnb@hgft.com", "isamnurisalib@armyspy.com", "marco_trevas@hotmail.com", "uhsuhsuhuhushushsuhsuhs@hotmail.com", "tacualpimpam@hotmail.com", "isameizoz@hotmail.com", "josebrito895@gmail.com", "lohaneammar@live.fr", "nwarh_ksa@hotmail.com", "waleeedkhaled162@gmail.com", "jeno@gmail.com", "ezfkzlmekfmlzefkmlzk@grr.la", "angelo.lombardo@sunrise.ch", "primigenium.magister@hotmail.com", "danielgonzalez252@hotmail.com", "an2003@mail.ru", "eva1287@hotmail.it", "edilf@yahoo.com.br", "fgdfgds@gmail.com", "camilleandrodin@gmail.com", "wossanakonate@yahoo.fr", "yuukw53@gmail.com", "mazy.mazy@sfr.fr", "lapequenyamega@hotmail.com", "muhammed_2321@yahoo.com", "slip@gmail.com", "edu_9415@hotmail.com", "rusekikoo@outlook.fr", "nono2000786@gmail.be", "neilsonsantana@gmail.com", "oliviamariaborges@gmail.com", "hgchjcy@oioi.com", "ahajjs@gmail.com", "adem.ali@gmail.com", "hhhhvvvv@uujj.com", "a11@gmail.com", "itsmina@hotmail.fr", "ma@hotmail.com", "fshshs@qfqf.com", "nadirhamido@gmail.com", "thyjfxxfy@gmail.com", "cvxb@frffgbb.com", "salamghazi@gmail.com", "olbriot@gmail.com", "khalidbe@gmail.com", "okan_yorukoglu@hotmail.com", "beloucv@gmail.com", "eder.brito15@gmail.com", "yertyshova@gmail.com", "gaylor.bouanna@orange.fr", "painkiller03@hotmail.com", "sesmariabebebco@gmail.com", "kara.kryptonian@gmail.com", "ertyu@fgh.dfg", "sdfgh@dfg.dfg", "nxx_@w.cn", "cool9090@gmail.com", "cortomaltese3@gmail.com", "jose.bressan@gmail.com", "marco.benjamin.avs@gmail.com", "nani123@gmail.com", "jessyca.rodriguez@orange.fr", "lbenadir@gmail.com", "vickyfleur29@gmail.com", "pesson75019@gmail.com", "josemanueldonet82@gmail.com", "plokhin@live.fr", "atimonautas@yahoo.com.br", "graphiculteur@gmail.com", "Abdulaziz_@hotmail.com", "selim.zazoua@gmail.com", "hajhaklH@gmail.com", "josemisc@hotmail.es", "srasra11@hotmail.com", "kjkfhdjghsdjn@ghgd.vvv", "badrjdvctr@hotmail.com", "ohh19@hotmail.com", "opes19@hotmail.com", "ohh21@hotmail.com", "linaldocard@gmail.com", "sergioalbertofuentes@gmail.com", "rekiba@hotmail.fr" };

        private void TestDechunk()
        {
            // detokenize
            var inputs = new string[]
            {
                "- Harry's your sister. - Look, what exactly am I supposed to be doing here?",
                "\"Piss off!\"",
                "- Sorry Mrs. Hudson, I'll skip the tea. Off out. - Both of you?",
                "I love playing half-life; that's just who I am!",
                "That's why I... have just begun to write a book.",
                "And they lived happily ever after...",
                "It's gonna be $1.5 sir."
            };

            // 
            var tokenizer = new EnglishMaximumEntropyTokenizer(currentDirectory + "../Resources/Models/EnglishTok.nbin");
            var chunker = new EnglishTreebankChunker(currentDirectory + "../Resources/Models/EnglishChunk.nbin");
            var dechunker = new RegexDictionaryDechunker();
            var detokienizer = new DictionaryDetokenizer();
            var englishPosPath = currentDirectory + "../Resources/Models/EnglishPOS.nbin";
            var tagDictPath = currentDirectory + "../Resources/Models/Parser/tagdict";
            var posTagger = new EnglishMaximumEntropyPosTagger(englishPosPath, tagDictPath);

            foreach (var input in inputs)
            {
                string[] tokens = tokenizer.Tokenize(input);
                string[] tags = posTagger.Tag(tokens);

                var chunks = chunker.GetChunks(tokens, tags);
                var chunksStrings = chunks
                    .Select(ch => detokienizer.Detokenize(ch.TaggedWords.Select(tw => tw.Word).ToArray()))
                    .ToArray();
                var output = dechunker.Dechunk(chunksStrings);
                Console.WriteLine("input: " + input);
                Console.WriteLine("chunks: " + string.Join(" | ", chunks));
                Console.WriteLine("ouput: " + output);
                Console.WriteLine("--");
            }
        }
    }
}
