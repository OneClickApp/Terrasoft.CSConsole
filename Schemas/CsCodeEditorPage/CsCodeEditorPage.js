define("CsCodeEditorPage", ["SourceCodeEditEnums", "SourceCodeEditGenerator", "css!CsCodeEditorPageCss"], function(sourceCodeEditEnums) {
	return {
		entitySchemaName: "",
		attributes: {},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {
			onProcessActionButtonClick: function(eventName, modelMethod, model, tag) {
				switch (tag) {
					case "RunCode":
						this.runCode();
						break;
					case "ClosePage":
                    	this.saveCode();
						this.callParent(arguments);
						break;
				}
			},
          
          	onRender: function() {
				this.callParent(arguments);
				this.getCode();
			},
			
			runCode: function(){
				this.saveCode();
				var code = this.get("CsCode");
				var encCode = this.b64Encode(code);
				var serviceData = { UserCode: encCode };
				var serviceClass = "ShellExecutor";
				
				var version = Terrasoft.coreVersion.split('.');
				version = Number.parseInt(version[0] + version[1]);
				
				if(version < 716){
					serviceClass = "ShellExecutorOld";
				}
				
				this.callServiceMethod(serviceClass, "Execute", function(responseObject, response){
					this.set("ResultStr", responseObject.ExecuteResult);
					document.getElementById("CsCodeEditorPageTabsTabPanel-tabpanel-items").children[1].click();
				}, serviceData, this);
			},
			
			b64Encode: function(str) {
			    return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g,
			        function toSolidBytes(match, p1) {
			            return String.fromCharCode('0x' + p1);
			    }));
			},
          	
          	saveCode: function(){
				localStorage.setItem("csCode", this.get("CsCode"));
			},
			
			getCode: function(){
				var csCode = localStorage.getItem("csCode");
				if(csCode){
					this.set("CsCode", csCode);
				}
			}
		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"name": "Button-913d25ceb96f4933ac04798a0fa9c70a",
				"values": {
					"itemType": 5,
					"id": "8c062282-eeee-43ab-9c46-c0c40dadb9f6",
					"style": "green",
					"tag": "RunCode",
					"caption": {
						"bindTo": "Resources.Strings.RunCodeButtonCaption"
					},
					"click": {
						"bindTo": "onProcessActionButtonClick"
					},
					"enabled": true
				},
				"parentName": "ProcessActionButtons",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "merge",
				"name": "Button-3a8ac667899d4aa68021a07eb1c7c49c",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.ClosePageButtonCaption"
					},
					"enabled": true
				}
			},
			{
				"operation": "merge",
				"name": "NewTab1",
				"values": {
					"order": 0
				}
			},
			{
				"operation": "insert",
				"name": "STRING4653bb23-87b8-4c6b-ac43-46c5914bce69",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 10,
						"column": 0,
						"row": 0,
						"layoutName": "NewTab1GridLayout1"
					},
					"bindTo": "CsCode",
					"labelConfig": {
						"visible": false
					},
					"enabled": true,
					"isRequired": true,
					"contentType": Terrasoft.ContentType.RICH_TEXT,
					"generator": "SourceCodeEditGenerator.generate",
					"language": sourceCodeEditEnums.Language.CSHARP,
					"classes": {
						"wrapClass": ["cs-code-editor"]
					}
				},
				"parentName": "NewTab1GridLayout1",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Tabf1e33e38TabLabel",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tabf1e33e38TabLabelTabCaption"
					},
					"items": [],
					"order": 1
				},
				"parentName": "Tabs",
				"propertyName": "tabs",
				"index": 1
			},
			{
				"operation": "insert",
				"name": "Tabf1e33e38TabLabelGroup7080d0ec",
				"values": {
					"caption": {
						"bindTo": "Resources.Strings.Tabf1e33e38TabLabelGroup7080d0ecGroupCaption"
					},
					"itemType": 15,
					"markerValue": "added-group",
					"items": []
				},
				"parentName": "Tabf1e33e38TabLabel",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "Tabf1e33e38TabLabelGridLayout4f0a51e0",
				"values": {
					"itemType": 0,
					"items": []
				},
				"parentName": "Tabf1e33e38TabLabelGroup7080d0ec",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "insert",
				"name": "STRINGb5bc6337-4f70-4cb0-bfc2-fdbfd3f308b8",
				"values": {
					"layout": {
						"colSpan": 24,
						"rowSpan": 10,
						"column": 0,
						"row": 0,
						"layoutName": "Tabf1e33e38TabLabelGridLayout4f0a51e0"
					},
					"bindTo": "ResultStr",
					"labelConfig": {
						"visible": false
					},
					"enabled": true,
					"contentType": 0
				},
				"parentName": "Tabf1e33e38TabLabelGridLayout4f0a51e0",
				"propertyName": "items",
				"index": 0
			},
			{
				"operation": "remove",
				"name": "Button-be6148b819154a0791eaee8f1635d859"
			}
		]/**SCHEMA_DIFF*/
	};
});
