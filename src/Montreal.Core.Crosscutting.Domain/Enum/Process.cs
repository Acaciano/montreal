using System.ComponentModel;

namespace Montreal.Core.Crosscutting.Domain.Enum
{
    public enum Process
    {
        [Description("CLASSIFICAÇÃO")]
        CLASSIFICATION = 1,

        [Description("ARMAZENAGEM")]
        STORAGE = 2,

        [Description("AUDITORIA")]
        AUDIT = 3,

        [Description("QUEBRA/SOBRA")]
        QUEBRA_SOBRA = 4,

        [Description("ESTADIA")]
        ESTADIA = 5,

        [Description("CARREGAMENTO")]
        CARREGAMENTO = 6,

        [Description("DESCARREGAMENTO")]
        DESCARREGAMENTO = 7,

        [Description("ELEVAÇÃO")]
        ELEVACAO = 8,

        [Description("TROCAR/TAG")]
        TAG_CHANGE = 9,

        [Description("ALTERAR/ETAPA")]
        STAGE_CHANGE = 10,

        [Description("BLOQUEAR/COLABORADOR")]
        EMPLOYEE_BLOCK = 11,
    }
}
