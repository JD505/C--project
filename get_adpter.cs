using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;



namespace ConsoleApp1
{
    class get_adpter
    {
        public static string Creat_txt(string pagina, int ip)
        {

            string inicio, fim;

            //opening file form ip's macnhine
           // string content = pagina;
           // string content1 = pagina;

            //var path = @"C:\Users\joaod\OneDrive\Área de Trabalho\Info_REal\i.ToString().HTML";
            string content = File.ReadAllText(pagina, Encoding.UTF8);
            string content1 = File.ReadAllText(pagina, Encoding.UTF8);

            string noEspace_1 = content1.Replace("</caption><tr> <td>", "\n-").Replace("</td> <td>", "").Replace("</td></tr><tr> <td>", "").Replace("</td></tr></table><BR><BR><table><caption>", ";");//conteudo sem espacos

            //text selected No
            inicio = "No  ";
            fim = "Mfg  ";

            string no = getBetween(noEspace_1, inicio, fim);

            //---------------------------------text selected MFG
            inicio = "Mfg  ";
            fim = "Adptr.";
            string mfg = getBetween(noEspace_1, inicio, fim);
   
            //----------------------------------text selected adpId
            inicio = "Adptr. ID";
            fim = "Actuations";
            string adpId = getBetween(noEspace_1, inicio, fim);
       
            // ----------------------------------text selected Actuations
            inicio = "Actuations";
            fim = "Insertions";
            string acts = getBetween(noEspace_1, inicio, fim);

            // ----------------------------------text selected Insertions
            inicio = "Insertions";
            fim = "Pass";
            string inserts = getBetween(noEspace_1, inicio, fim);
       
            // ----------------------------------text selected Pass
            inicio = "Pass";
            fim = "Fail";
            string pass_t = getBetween(noEspace_1, inicio, fim);
          
            // ----------------------------------text selected Fail
            inicio = "Fail";
            fim = "Clean Alert";
            string Fail_s = getBetween(noEspace_1, inicio, fim);
        
            // ----------------------------------text selected Clean Alert
            inicio = "Clean Alert";
            fim = "Adptr. Life";
            string clean = getBetween(noEspace_1, inicio, fim);

            // ----------------------------------text selected Adptr. Life
            inicio = "Adptr. Life";
            fim = "Socket";
            string life = getBetween(noEspace_1, inicio, fim);            

            // ----------------------------------text selected Socket
            inicio = "Socket";
            fim = "Yield ";
            string socket_T = getBetween(noEspace_1, inicio, fim);

            // ----------------------------------text selected Yield 
            inicio = "Yield";
            fim = ";";
            string yield_t = getBetween(noEspace_1, inicio, fim);

            string time = DateTime.Now.ToString("dd-MM-yyyy-HH-mm");
            string detailText = String.Concat("No ", no, "\nMFG ", mfg, "\nAdptr. ID ", adpId, "\nActuations ", acts, "\nInsertions ", inserts, "\nPass ", pass_t, "\nFail ", Fail_s, "\nClean Alert ", clean, "\nlife ", life, "\nsocket ", socket_T, "\nYield ", yield_t, "\ntime ", time, "\n");

            // count the number of sockets
            int trueCount = Int32.Parse(socket_T);

            int i;
            string[] socket = new string[1000];

            for (i = 1; i <= trueCount; i++)///////////////////////////////////////////////////////////////
            {

                //text selected start and finish
                inicio = "Socket " + i.ToString();
                fim = "Socket ";// + (i + 1).ToString();

                string noEspace = content.Replace("</td> <td>", "\n-").Replace("</td> </tr><tr><td>", ";").Replace("</td> </tr></tbody></table></BODY></HTML>", "; Socket  ");//conteudo +-sem codigo html
                string texto = getBetween(noEspace, inicio, fim);

                string insertions = getBetween(texto, "-", "-");//primeira linha
                string noIsertions = removeExtra(texto, insertions, "-"); // new string sem primeira linha

                string pass = getBetween(noIsertions, "-", "-");
                string noPass = removeExtra(noIsertions, pass, "-");

                string fail = getBetween(noPass, "-", "-");
                string noFail = removeExtra(noPass, fail, "-");

                string cleanCount = getBetween(noFail, "-", "-");
                string noCleanCount = removeExtra(noFail, cleanCount, "-");

                string yield = getBetween(noCleanCount, "-", ";");
                //string noyield = removeExtra(noCleanCount, yield, "-");
               

                socket[i] = String.Concat(socket[i - 1], inicio + "\n", "insertions " + insertions, "pass " + pass, "fail " + fail, "cleanCount " + cleanCount, "yield " + yield + ";\n");

                //criar ficheiro .txt
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Info\\" + no + "_" + time + ".txt", detailText + socket[i]);
            }
            return "1";
        }

        public static string getBetween(string strSource, string strStart, string strEnd) //function for get text between
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                if (End < 0)
                {
                    End = 0;
                }
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }

        
        public static string removeExtra(string strSource, string strStart, string strEnd) //function for get text between
        {
            int Start, End, str;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                str = strSource.Length;
                return strSource.Substring(End, str - End);


            }
            else
            {
                return "";
            }


        }
    }
}
