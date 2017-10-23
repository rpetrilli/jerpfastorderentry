using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;


namespace fastOrderEntry.Models
{
    public class PetLineContext: DbContext
    {
        static PetLineContext()
        {
            //non modifica lo schema del database
            Database.SetInitializer<PetLineContext>(null);
        }

        public PetLineContext() : base(nameOrConnectionString: "DefaultConnectionString") { }

        public virtual DbSet<CodiceIva> codiceIva { get; set; }
        public virtual DbSet<CondizionePag> condizionePag { get; set; }
        public virtual DbSet<Impostazioni> impostazioni { get; set; }
        public virtual DbSet<ma_articoli_soc> ma_articoli_soc { get; set; }
        public virtual DbSet<da_listini_articolo> da_listini_articolo { get; set; }
        public virtual DbSet<da_sconti_articolo> da_sconti_articolo { get; set; }
        public virtual DbSet<CondizioneCliente> CondizioneCliente { get; set; }
        public virtual DbSet<da_listini_cliente> da_listini_cliente { get; set; }
        public virtual DbSet<da_sconti_cliente_articolo> da_sconti_cliente_articolo { get; set; }
        public virtual DbSet<Agenti> agenti { get; set; }
        public virtual DbSet<UtenteModel> utenti { get; set; }




        protected override void OnModelCreating(DbModelBuilder modelBulider)
        {
            modelBulider.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBulider);
        }
    }
}