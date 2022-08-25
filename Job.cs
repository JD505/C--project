using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Reflection;


namespace ConsoleApp1
{
    public class Job
    {
        public class repetir
        {
          
            public string name_job { get; set; }
            public string data_end { get; set; }
        }
        public struct jobInfo
        {
            public int contagem;
            public int job_name;         
            public DateTime data_Inicio;
            public DateTime data_Fim;
            public int numero_Jobs;
            public string machine_Name;
            public string type;

        }


        jobInfo[] job;
        
        public void getJob()
        {
            string linha = string.Empty;
            string inicio = string.Empty;
            string fim = string.Empty;
            string dateAux = string.Empty;
            int size = 0;

           // jobInfo[] job;
            List<string> logList;

            if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\job_list"))
            {
                 logList = Deserialize_List();            
            }
            else
            {
                 logList = new List<string>();
            }

            // Create a reference to the current directory            
            if (Directory.Exists(@"C:\Users\joaod\OneDrive\Área de Trabalho\log"))
            {
                DirectoryInfo di = new DirectoryInfo(@"C:\Users\joaod\OneDrive\Área de Trabalho\log\");
                FileInfo[] folder = di.GetFiles();

                List<string> logList_aux = new List<string>();

                // open current directory
                foreach (FileInfo fiName in folder)
                {
                    long txtSize = fiName.Length;

                    if (txtSize >= 1048580)
                    {
                        logList_aux.Add(fiName.Name);
                    }


                }

                IEnumerable<string> lista_New = logList_aux.Except(logList);

                foreach (var nome in lista_New)
                {

                    var path = @"C:\Users\joaod\OneDrive\Área de Trabalho\log\" + nome;
                    int lineCount = File.ReadLines(path).Count();

                    //guardar numero de jobs iniciados
                    using (StreamReader inputFile = new StreamReader(path))
                    {
                        for (int i = 1; i < lineCount; i++)
                        {
                            linha = inputFile.ReadLine();

                            if (linha.Contains("InitSystem started"))
                            {
                                size++;
                            }

                        }

                    }

                    if (File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\job_Specs"))
                    {
                        jobInfo[] auxJob = Deserialize();
                        job = new jobInfo[size + auxJob[0].numero_Jobs];

                        for (int i = 0; i < auxJob[0].numero_Jobs; i++)
                        {

                            job[i].contagem = auxJob[i].contagem;
                            job[i].job_name = auxJob[i].job_name;
                            job[i].data_Inicio = auxJob[i].data_Inicio;
                            job[i].data_Fim = auxJob[i].data_Fim;
                            job[i].machine_Name = auxJob[i].machine_Name;
                            job[i].type = auxJob[i].type;
                        }

                        job[0].numero_Jobs = auxJob[0].numero_Jobs;
                    }
                    else
                    {
                        job = new jobInfo[size];
                        job[0].numero_Jobs = 0;

                    }

                    using (StreamReader inputFile = new StreamReader(path))
                    {
                        string type = string.Empty;

                        for (int i = 1; i < lineCount; i++)
                        {
                            linha = inputFile.ReadLine();

                            if (linha.Contains("InitSystem started"))    //procurar inicio dos jobs                 
                            {

                                dateAux = get_adpter.getBetween(linha, "", ";");
                                dateAux = dateAux.Remove(dateAux.Length - 4);
                                try
                                {
                                    // guardar valores do inicio do job  
                                    job[job[0].numero_Jobs].data_Inicio = (DateTime.ParseExact(dateAux, "dd.MM.yy HH:mm:ss", null));
                                    job[job[0].numero_Jobs].job_name = Int32.Parse(get_adpter.getBetween(linha, ";", ";"));

                                    // add machine name
                                    job[job[0].numero_Jobs].machine_Name = Environment.MachineName;

                                    //incrementar total de jobs
                                    job[0].numero_Jobs++;
                                }
                                catch
                                { //falhou 
                                }
                            }
                            if (linha.Contains("g2511.txt"))
                            {
                                type = "g2511";
                            }
                            else if (linha.Contains("g3156.txt"))
                            {
                                type = "g3156";
                            }
                            if (linha.Contains("finished;"))
                            {

                                dateAux = get_adpter.getBetween(linha, "", ";");
                                dateAux = dateAux.Remove(dateAux.Length - 4);

                                int jobNameAux = 0;
                                DateTime dataFim_Aux = new DateTime();

                                DateTime menorData = new DateTime();
                                bool menor_flag = false;

                                for (int j = 0; j < job[0].numero_Jobs; j++)
                                {

                                    try
                                    {
                                        jobNameAux = Int32.Parse(get_adpter.getBetween(linha, ";", ";"));
                                        dataFim_Aux = (DateTime.ParseExact(dateAux, "dd.MM.yy HH:mm:ss", null));
                                    }
                                    catch { }

                                    if (jobNameAux == job[j].job_name && job[j].contagem == 0 && DateTime.Compare(job[j].data_Inicio, dataFim_Aux) < 0)
                                    {
                                        if (menor_flag == false)
                                        {
                                            menor_flag = true;
                                            menorData = job[j].data_Inicio;
                                        }

                                        if (DateTime.Compare(job[j].data_Inicio, menorData) < 0)
                                        {
                                            menorData = job[j].data_Inicio;
                                        }

                                    }
                                }

                                for (int j = 0; j < job[0].numero_Jobs; j++)
                                {
                                    if (jobNameAux == job[j].job_name && job[j].contagem == 0 && DateTime.Compare(job[j].data_Inicio, menorData) == 0)
                                    {

                                        try
                                        {
                                            job[j].data_Fim = dataFim_Aux;
                                            job[j].contagem = Int32.Parse(get_adpter.getBetween(linha, "Pass Limit: ", " (VPE) finished"));
                                            job[j].type = type;
                                        }
                                        catch { }
                                    }
                                }


                            }
                        }

                    }


                    Serialize(job);

                    logList.Add(nome);
                    Serialize_List(logList);
                }
            }
            else
            {
                Console.WriteLine("ERROR LOADING JOB SPECS!\nPlease insert machine log files on Correct directory: \nFile example: log-2020-02-27-19-03 \n ");
            }
            Console.ReadKey();
        }


        public static void Serialize(jobInfo[] input)
        {
            if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + " \\Info"))
            {
                Directory.CreateDirectory((Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + " \\Info"));
            }

            var serializer = new XmlSerializer(input.GetType());
            var sw = new StreamWriter(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\job_Specs");
            serializer.Serialize(sw, input);
            sw.Close();
        }
        public static void Serialize_List(List<string> input)
        {
            if (!File.Exists((Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\job_list")))
            {
                Directory.CreateDirectory((Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + " \\Info"));
            }
            var serializer = new XmlSerializer(input.GetType());
            var sw = new StreamWriter(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\job_list");
            serializer.Serialize(sw, input);
            sw.Close();
        }

        public static jobInfo[] Deserialize()
        {
            var stream = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\job_Specs");
            var ser = new XmlSerializer(typeof(jobInfo[]));
            object obj = ser.Deserialize(stream);
            stream.Close();
            return (jobInfo[])obj;
        }
        public static List<string> Deserialize_List()
        {

            var stream = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\job_list");
            var ser = new XmlSerializer(typeof(List<string>));
            object obj = ser.Deserialize(stream);
            stream.Close();
            return (List<string>)obj;
        }
    }
}
