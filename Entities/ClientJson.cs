namespace OracleConfig.Entities
{
    class ClientJson
    {
        public string Cliente { get; set; }
        public string DirDump { get; set; }
        public string FileNameDump { get; set; }
        public string UserNameBase { get; set; }
        public string PasswordBase { get; set; }
        public string TnsNames { get; set; }
        public string OraInst { get; set; }
        public string BaseIpAddress { get; set; }
        public int BasePort { get; set; }
        public string[] UsersDump { get; set; }
        public string PasswordUser { get; set; }
        public string Tablespace { get; set; }
        public string TablespaceDir { get; set; }
        public bool RemapTablespaceHabilitado { get; set; }
        public string[] RemapTablespace { get; set; }

        public ClientJson(){}
    }
}