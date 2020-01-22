namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SaveItem
{
    using Sitecore.ExperienceEditor.Speak.Server.Contexts;
    using Sitecore.ExperienceEditor.Speak.Server.Requests;
    using Sitecore.ExperienceEditor.Speak.Server.Responses;
    using Sitecore.ExperienceEditor.Switchers;
    using Sitecore.Caching;
    using Sitecore.Data;
    using Sitecore.Globalization;
    using Sitecore.Pipelines;
    using Sitecore.Pipelines.Save;
    using Sitecore.Diagnostics;
    using System.Linq;

    public class CallServerSavePipeline : PipelineProcessorRequest<PageContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            PipelineProcessorResponseValue pipelineProcessorResponseValue = new PipelineProcessorResponseValue();
            Pipeline pipeline = PipelineFactory.GetPipeline("saveUI");
            pipeline.ID = ShortID.Encode(ID.NewID);
            SaveArgs saveArgs = base.RequestContext.GetSaveArgs();

            using (new ClientDatabaseSwitcher(base.RequestContext.Item.Database))
            {
                pipeline.Start(saveArgs);
                CacheManager.GetItemCache(base.RequestContext.Item.Database).Clear();
                pipelineProcessorResponseValue.AbortMessage = Translate.Text(saveArgs.Error);
                SaveArgs.SaveItem contextSaveItem = GetContextSaveItem(saveArgs, base.RequestContext.Item.ID);
                if (contextSaveItem != null && contextSaveItem.Version != null)
                {
                    pipelineProcessorResponseValue.Value = contextSaveItem.Version.Number;
                }
                return pipelineProcessorResponseValue;             
            }
        }
        protected virtual SaveArgs.SaveItem GetContextSaveItem(SaveArgs saveArgs, ID itemId)
        {
            Assert.ArgumentNotNull(saveArgs, "saveArgs");
            Assert.ArgumentNotNull(itemId, "itemId");
            return saveArgs.Items.FirstOrDefault((SaveArgs.SaveItem si) => si.ID == itemId);
        }
    }
}