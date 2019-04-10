using System;
using System.IO;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

using OracleConfig.Entities;
using OracleConfig.Entities.Exceptions;

namespace OracleConfig.Services
{
    class OracleCommandService : ICommandService
    {
        private string _pathCfg;
        private ClientJson _clientJson;

        public OracleCommandService(string client)
        {  
            if (string.IsNullOrWhiteSpace(client)){
                throw new DomainException("Undefined name client.");
            }

            _pathCfg += Path.Combine(Directory.GetCurrentDirectory(), "client_json", client + ".json");

            if (!File.Exists(_pathCfg))
            {
                throw new DomainException($"File not found. {_pathCfg}");
            }
        }

        public void CreateConfig()
        {
            string pathCmd = Path.Combine(Directory.GetCurrentDirectory(), "client_command");

            if (!Directory.Exists(pathCmd))
            {
                throw new DomainException($"Directory not found. {pathCmd}");
            }

            using (StreamReader file = File.OpenText(_pathCfg))
            {
                _clientJson = JsonConvert.DeserializeObject<ClientJson>(file.ReadToEnd());
            }

            

            using (var memory = new MemoryStream())
            using (var textWrite = new StreamWriter(memory))
            {
                string datahora = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
                string fileCommand = Path.Combine(pathCmd, $"command_{_clientJson.Cliente.ToLower()}_{datahora}.txt");

                string tableSpaceName = _clientJson.Cliente.ToUpper();

                string tnsNames = _clientJson.TnsNames.ToUpper();

                StringBuilder linhaCmd = new StringBuilder();

                linhaCmd.AppendLine("( Abrir o prompt com permissão de admin )");
                linhaCmd.AppendLine(string.Empty.PadRight(111, '-'));

                linhaCmd.AppendLine();

                linhaCmd.AppendLine("( Editando o tnsnames.ora )");
                linhaCmd.AppendLine(string.Empty.PadRight(111, '-'));

                linhaCmd.AppendLine();

                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));
                linhaCmd.AppendLine(tnsNames + " =");
                linhaCmd.AppendLine("  (DESCRIPTION =");
                linhaCmd.AppendLine("    (SOURCE_ROUTE = on)");
                linhaCmd.AppendLine("    (CONNECT_TIMEOUT = 10) (RETRY_COUNT = 3) (RETRY_DELAY = 2)");
                linhaCmd.AppendLine("    (COMPRESSION = on)");
                linhaCmd.AppendLine("    (COMPRESSION_LEVELS = (LEVEL = high))");
                linhaCmd.AppendLine("    (ADDRESS = (PROTOCOL = TCP) (HOST = " + _clientJson.BaseIpAddress + ") (PORT = " + _clientJson.BasePort + ") (IP = first))");
                linhaCmd.AppendLine("    (CONNECT_DATA = (SERVER = DEDICATED) (SID = " + _clientJson.OraInst + "))");
                linhaCmd.AppendLine("  )");
                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));

                linhaCmd.AppendLine();

                linhaCmd.AppendLine("( Editando arquivo sqlora.net )");
                linhaCmd.AppendLine(string.Empty.PadRight(111, '-'));

                linhaCmd.AppendLine();

                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));
                linhaCmd.AppendLine("NAMES.DIRECTORY_PATH= (TNSNAMES, HOSTNAME, LDAP)");
                linhaCmd.AppendLine("##NAMES.DEFAULT_DOMAIN = ORACLE.COM");
                linhaCmd.AppendLine("TRACE_LEVEL_CLIENT = ON");
                linhaCmd.AppendLine("##SQLNET.EXPIRE_TIME = 30");
                linhaCmd.AppendLine("SQLNET.IDENTIX_FINGERPRINT_DATABASE = FINGRDB");
                linhaCmd.AppendLine("AUTOMATIC_IPC = ON");
                linhaCmd.AppendLine("SQLNET.EXPIRE_TIME = 0");
                linhaCmd.AppendLine("SQLNET.AUTHENTICATION_SERVICES = (ALL)");
                linhaCmd.AppendLine("SQLNET.CRYPTO_CHECKSUM_CLIENT = ACCEPTED");
                linhaCmd.AppendLine("##TNSPING.TRACE_DIRECTORY = /oracle/traces");
                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));

                linhaCmd.AppendLine();
                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));

                linhaCmd.AppendLine(@"sqlplus /nolog");
                linhaCmd.AppendLine(@"connect sys/viewinfo@" + tnsNames + " as sysdba;");
                linhaCmd.AppendLine("grant all privileges to system with admin option;");
                linhaCmd.AppendLine("grant resource, connect, dba, advisor, sysdba, sysoper to system with admin option;");
                linhaCmd.AppendLine("quit");

                linhaCmd.AppendLine();
                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));


                linhaCmd.AppendLine(@"sqlplus " + _clientJson.UserNameBase + @"/" + _clientJson.PasswordBase + "@" + tnsNames + " as sysdba");
                linhaCmd.AppendLine("set linesize 500;");
                linhaCmd.AppendLine("set pagesize 1000;");
                linhaCmd.AppendLine();

                foreach (string usuario in _clientJson.UsersDump)
                {
                    linhaCmd.AppendLine($"drop user {usuario} cascade;");
                }

                linhaCmd.AppendLine("commit;");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));

                linhaCmd.AppendLine("select distinct"); 
                linhaCmd.AppendLine("  owner"); 
                linhaCmd.AppendLine("from");
                linhaCmd.AppendLine("  dba_tables"); 
                linhaCmd.AppendLine("where"); 
                linhaCmd.AppendLine($"  tablespace_name='{tableSpaceName}';");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine("select"); 
                linhaCmd.AppendLine("  owner,"); 
                linhaCmd.AppendLine("  constraint_name,");
                linhaCmd.AppendLine("  table_name,");
                linhaCmd.AppendLine("  index_owner,");
                linhaCmd.AppendLine("  index_name");
                linhaCmd.AppendLine("from"); 
                linhaCmd.AppendLine("  dba_constraints"); 
                linhaCmd.AppendLine("where"); 
                linhaCmd.AppendLine($"  (index_owner,index_name) in (select owner, index_name from dba_indexes where tablespace_name ='{tableSpaceName}')");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine($"drop tablespace {tableSpaceName} including contents and datafiles cascade constraints;");
                linhaCmd.AppendLine("commit;");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine($"create bigfile tablespace {tableSpaceName} datafile '{_clientJson.TablespaceDir}\\{tableSpaceName}.DBF' size 10000M autoextend on;");
                linhaCmd.AppendLine("commit;");

                linhaCmd.AppendLine();

                
                linhaCmd.AppendLine("select"); 
                linhaCmd.AppendLine("  username,"); 
                linhaCmd.AppendLine("  default_tablespace,"); 
                linhaCmd.AppendLine("  temporary_tablespace"); 
                linhaCmd.AppendLine("from"); 
                linhaCmd.AppendLine("  dba_users"); 
                linhaCmd.AppendLine("where"); 
                linhaCmd.Append("  username in (");

                var ultimoUser = _clientJson.UsersDump.Last();

                foreach (string user in _clientJson.UsersDump)
                {
                    linhaCmd.Append($"'{user}'");
                    if (ultimoUser != user)
                    {
                        linhaCmd.Append(", ");
                    }
                }

                linhaCmd.AppendLine(");");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine("SELECT");
                linhaCmd.AppendLine("  TABLESPACE_NAME,");
                linhaCmd.AppendLine("  ROUND((USED_SPACE * (8192 / 1048576))) AS USED_SPACE, --Espaço em MB");
                linhaCmd.AppendLine("  ROUND((TABLESPACE_SIZE * (8192 / 1048576))) AS TABLESPACE_SIZE, --Espaço em MB");
                linhaCmd.AppendLine("  ROUND(((USED_SPACE / TABLESPACE_SIZE) * 100)) || '%' AS PERCENT_USED");
                linhaCmd.AppendLine("FROM");
                linhaCmd.AppendLine("  DBA_TABLESPACE_USAGE_METRICS");
                linhaCmd.AppendLine("WHERE");
                linhaCmd.AppendLine($"  TABLESPACE_NAME IN('SYSTEM', '{tableSpaceName}'");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));
                foreach (string user in _clientJson.UsersDump)
                {
                    linhaCmd.AppendLine($"create user {user} default tablespace {tableSpaceName} identified by {_clientJson.PasswordUser} quota unlimited on users;");
                    linhaCmd.AppendLine($"grant all privileges to {user} with admin option;");
                    linhaCmd.AppendLine($"grant resource, connect, dba, advisor, sysdba, sysoper to {user} with admin option;");
                    linhaCmd.AppendLine();
                }

                linhaCmd.AppendLine("commit;");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine($"drop directory {_clientJson.Cliente};");
                linhaCmd.AppendLine("commit;");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine($"create directory {_clientJson.Cliente} as '{_clientJson.DirDump}';");
                linhaCmd.AppendLine("commit;");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));
                linhaCmd.AppendLine($"impdp {_clientJson.UserNameBase}/{_clientJson.PasswordBase}@{tnsNames} directory={_clientJson.Cliente} dumpfile={_clientJson.FileNameDump} logfile=IMPDP_{_clientJson.Cliente}_{datahora}.log table_exists_action=replace transform=segment_attributes:n {Remap_tablespace(_clientJson)} full=y data_options=SKIP_CONSTRAINT_ERRORS");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine(string.Empty.PadRight(111, '='));
                linhaCmd.AppendLine($"imp {_clientJson.UserNameBase}/{_clientJson.PasswordBase}@{tnsNames} fromuser={_clientJson.UsersDump[0]} touser={_clientJson.UsersDump[0]} file={_clientJson.FileNameDump} log=IMP_{_clientJson.Cliente}_{datahora}.log ignore=y commit=y");

                linhaCmd.AppendLine();

                linhaCmd.AppendLine($"select max(dt_movimen) from ts_movat");

                // Executando a gravação do arquivo de comando.
                textWrite.WriteLine(linhaCmd);
                textWrite.Flush();

                using (StreamWriter fileCmd = File.CreateText(fileCommand))
                {
                    fileCmd.BaseStream.Write(memory.ToArray(), 0, (int)memory.Length);
                }
            }
        }

        static string Remap_tablespace(ClientJson config)
        {
            string linha;
            linha = "remap_tablespace=(";

            var ultimo = config.RemapTablespace.Last();

            foreach (var table in config.RemapTablespace)
            {
                linha += table + ":" + config.Tablespace;
                if(table != ultimo)
                {
                    linha += ", ";
                }
            }

            linha += ") ";

            if(!config.RemapTablespaceHabilitado)
            {
                linha = String.Empty;
            }

            return linha;
        }
    }
}