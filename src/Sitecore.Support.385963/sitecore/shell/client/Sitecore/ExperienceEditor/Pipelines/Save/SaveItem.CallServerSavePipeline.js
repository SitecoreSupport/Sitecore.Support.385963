define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
  return ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.Save.CallServerSavePipeline", function (response) {
    ExperienceEditor.getContext().isModified = false;
    if (!ExperienceEditor.getContext().instance.disableRedirection) {
		var responseValue = response.responseValue;
		if(responseValue.value != null 
		&& responseValue.value != undefined
		&& responseValue.value.url != null
		&& responseValue.value.url != undefined)
		{
			window.parent.location.href = responseValue.value.url;
		} else {		
			window.parent.location.reload();
		}
    }
    ExperienceEditor.getContext().instance.disableRedirection = false;
  });
});