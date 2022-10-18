using System.ComponentModel;

namespace Utils.Enums
{
    public enum CondicaoEnum
    {
        [Description("Produto sem uso")]
        Novo = 1,

        [Description("Produto em condição excelente, praticamente novo")]
        Excelente = 2,

        [Description("Produto com desgaste natural de uso e imperfeições visuais, mas ainda ok")]
        Usado = 3,

        [Description("Produtos com marcas, desgastes e/ou imperfeições devido ao uso")]
        Regular = 4,

        [Description("Produto em mal estado ou disfuncional")]
        Disfuncional = 5
    }
}
