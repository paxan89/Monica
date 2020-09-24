using Autofac;
using Frgo.Dohod.DbModel.DataAdapter.Import;
//using Frgo.Dohod.DbModel.DataAdapter.LevelOrg;
using Frgo.Dohod.DbModel.DataAdapter.Migrate;
using Frgo.Dohod.DbModel.DataAdapter.Parse.File;
using Frgo.Dohod.DbModel.DataAdapter.Parse.Format;
using Frgo.Dohod.DbModel.DataAdapter.Parse.Preview;
using Frgo.Dohod.DbModel.DataAdapter.Settings;
using Frgo.Dohod.DbModel.DataAdapter.Settings.Resources;
using Frgo.Dohod.DbModel.Interfaces.Adapters;
using Frgo.Dohod.DbModel.Interfaces.Adapters.Settings;
using Frgo.Dohod.DbModel.Manager.Parse;
using Monica.Core.Attributes;


namespace Frgo.Dohod.DbModel.Autofac
{
    /// <summary>
    /// Модуль IoC контейнера
    /// </summary>
    [CommonModule]
    public class DohodDbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ImportAdapter>().As<IImportAdapter>();
            builder.RegisterType<MigrateFoxToMySql>().As<IMigrateFoxToMySql>();

            builder.RegisterType<LevelOrgAdapter>().As<ILevelOrgAdapter>();
            builder.RegisterType<RolesAdapter>().As<IRolesAdapter>();
            builder.RegisterType<TypeLevelAdapter>().As<ITypeLevelAdapter>();
            builder.RegisterType<UsersAdapter>().As<IUsersAdapter>();
            builder.RegisterType<BtnsAdapter>().As<IBtnsAdapter>();
            builder.RegisterType<ModesAdapter>().As<IModesAdapter>();

            builder.RegisterType<ParseFileFkDocx>().Named<IParseFile>("FKDOCX");
            builder.RegisterType<ParseFileXl>().Named<IParseFile>("MFXL");
            builder.RegisterType<ParseFileFk>().Named<IParseFile>("FKTX");

            builder.RegisterType<ParserFkDocs00>().Named<IParserFormat>("FKDOCX00");
            builder.RegisterType<ParserMfXlVp>().Named<IParserFormat>("MFXLVP");
            builder.RegisterType<ParserFkTxBd>().Named<IParserFormat>("FKTXBD");
            builder.RegisterType<ParserFkTxVt>().Named<IParserFormat>("FKTXVT");
            builder.RegisterType<ParserFkTxSf>().Named<IParserFormat>("FKTXSF");
            builder.RegisterType<ParserFkTxZf>().Named<IParserFormat>("FKTXZF");
            builder.RegisterType<ParserFkTxIp>().Named<IParserFormat>("FKTXIP");
            builder.RegisterType<ParserFkTxNp>().Named<IParserFormat>("FKTXNP");
            builder.RegisterType<ParserFkTxCe>().Named<IParserFormat>("FKTXCE");
            builder.RegisterType<ParserFkTxSe>().Named<IParserFormat>("FKTXSE");

            builder.RegisterType<PreviewMfXlVp>().Named<IPreviewFormat>("MFXLVP");
            builder.RegisterType<PreviewFkTxBd>().Named<IPreviewFormat>("FKTXBD");
            builder.RegisterType<PreviewFkTxVt>().Named<IPreviewFormat>("FKTXVT");
            builder.RegisterType<PreviewFkTxSf>().Named<IPreviewFormat>("FKTXSF");
            builder.RegisterType<PreviewFkTxZf>().Named<IPreviewFormat>("FKTXZF");
            builder.RegisterType<PreviewFkTxIp>().Named<IPreviewFormat>("FKTXIP");
            builder.RegisterType<PreviewFkTxNp>().Named<IPreviewFormat>("FKTXNP");
            builder.RegisterType<PreviewFkTxCe>().Named<IPreviewFormat>("FKTXCE");
            builder.RegisterType<PreviewFkTxSe>().Named<IPreviewFormat>("FKTXSE");


           
        }
    }
}
