using System.ComponentModel;

namespace Montreal.Core.Crosscutting.Domain.Enum
{
    public enum DocTypeElevation
    {
        [Description("LAUDO DE CLASSIFICAÇÃO")]
        LaudoDeClassificacao = 1,
        [Description("CARTA PROTESTO")]
        CartaProtesto = 2,
        [Description("COMPROVANTE TAXA CDP")]
        ComprovanteTaxaCDP= 3,
        [Description("DECLARAÇÃO DE CIÊNCIA DAS NORMAS BRASILEIRAS DE SEGURANÇA PÚBLICA PORTUÁRIA")]
        DeclaracaoDeCienciaDasNormasBrasileirasSegurancaPublicaPortuaria = 4,
        [Description("DECLARAÇÃO MARÍTIMA DE SAÚDE")]
        DeclaracaoMaritimaDeSaude = 5,
        [Description("DU-E (NOMEAÇÃO)")]
        DueNomeacao = 6,
        [Description("DU-E AVERBADA")]
        DueAverbada = 7,
        [Description("LISTA DE TRIPULANTES")]
        ListaDeTripulantes = 8,
        [Description("MATES RECEIPT")]
        MatesReceipt = 9,
        [Description("NOR")]
        Nor = 10,
        [Description("PLANO DE CARGA")]
        PlanoDeCarga = 11,
        [Description("POST OF CALL")]
        PostDeCall = 12,
        [Description("SEQUÊNCIA DE EMBARQUE")]
        SequenciaDeEmbarque = 13,
        [Description("SHIP PARTICULARS")]
        ShipParticulars = 14,
        [Description("SOF HBSA")]
        SofHBSA = 15,
        [Description("SOF AGÊNCIA MARÍTIMA")]
        SofAgenciaMaritima = 16
    }
}
