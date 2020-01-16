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
    using Sitecore.Links;
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
                if (Sitecore.Context.PageMode.IsExperienceEditor && !Sitecore.Context.User.IsAdministrator && saveArgs.SavedItems.Count > 0)
                {
                    var item = base.RequestContext.Item;
                    if (item!=null && !string.IsNullOrEmpty(item[FieldIDs.Workflow]) && (item.State.GetWorkflowState() == null || item.State.GetWorkflowState().FinalState))
                    {
                        pipelineProcessorResponseValue.Value = new { url = LinkManager.GetItemUrl(item) };
                    }
                }
                return pipelineProcessorResponseValue;
            }
        }
    }
}