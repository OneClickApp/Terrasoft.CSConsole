define("SystemDesigner", ["SystemDesignerResources", "ProcessModuleUtilities"],
	function(resources, ProcessModuleUtilities) {
	return {
		attributes: {},
		methods: {
			onCsConsoleProcessLinkClick: function() {
				ProcessModuleUtilities.executeProcess({
                    sysProcessName: "CsConsole"
                });
				return false;
			}
		},
		diff: [
			{
				"operation": "insert",
				"propertyName": "items",
				"parentName": "ConfigurationTile",
				"name": "CsConsoleProcessLink",
				"values": {
					"itemType": Terrasoft.ViewItemType.LINK,
					"caption": {"bindTo": "Resources.Strings.CsConsoleProcessLinkCaption"},
					"click": {"bindTo": "onCsConsoleProcessLinkClick"}
				}
			}
		]
	};
});