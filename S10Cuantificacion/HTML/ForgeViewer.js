// getParameterByName from https://stackoverflow.com/a/901144
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

var urn = getParameterByName('URN');
var token = getParameterByName('Token');

if (urn != null && token != null) {
    showModel(urn, token);
}

// Viewer tutorial
var viewer;

function showModel(urn, token) {
    debugger;
    var options = {
        env: 'AutodeskProduction',
        accessToken: token
    };
    var documentId = 'urn:' + urn;   
    //Autodesk.Viewing.Initializer(options, function onInitialized() {
        Autodesk.Viewing.Initializer(options, () => {
            const config = {
                extensions: ['Autodesk.VisualClusters', 'Autodesk.DocumentBrowser', 'MenuContextual']
            };
        viewer = new Autodesk.Viewing.GuiViewer3D(document.getElementById('MyViewerDiv'));
        viewer.start();
        Autodesk.Viewing.Document.load(documentId, onDocumentLoadSuccess, onDocumentLoadFailure);
        viewer.autocam.shotParams.destinationPercent = 3;
            viewer.autocam.shotParams.duration = 3;



           /* var vsmd = new Autodesk.Viewing.ViewerScreenModeDelegate(viewer);
            var oldMode = vsmd.getMode();
            console.log(oldMode);//kFullScreen, kFullBrowser, kNormal

            if (vsmd.isModeSupported(Autodesk.Viewing.Viewer
                .ScreenMode.kFullBrowser)) {
                var newMode = Autodesk.Viewing.Viewer.ScreenMode.kFullBrowser;
                vsmd.doScreenModeChange(oldMode, newMode)
                //vsmd.setMode(newMode);

            }
            else {
                console.log('ScreenMode.kFullBrowser not supported');
            }*/


    });
}

function onDocumentLoadSuccess(doc) {
    //var viewables = (viewableId ? doc.getRoot().findByGuid(viewableId) : doc.getRoot().getDefaultGeometry());
    const geom = doc.getRoot().getDefaultGeometry();
    viewer.loadDocumentNode(doc, geom);
    viewer.loadExtension('MenuContextual');    
    viewer.loadExtension('Autodesk.VisualClusters');    
    viewer.loadExtension('Autodesk.DocumentBrowser');    
}

function onDocumentLoadFailure(viewerErrorCode) {
    alert('File not translated or not viewable');
    console.error('onDocumentLoadFailure() - errorCode:' + viewerErrorCode);
}

function highlightRevit(idsRevit) {
    // Every Forge Viewer model has an ‘ExternalId Mapping’
    // this mapping is an object that has as keys the
    this.viewer.model.getExternalIdMapping((mapping) => {
        this.configureElementByUniqueIdAndMapping(idsRevit, mapping);
    });
}
function configureElementByUniqueIdAndMapping(idsRevit, mapping) {
    var elementsDbId = [];
    var idsRevitArray = idsRevit.split(',');
    for (var uniqueId in idsRevitArray) {
        const elementDbId = mapping[idsRevitArray[uniqueId]];
        if (elementDbId) {
            elementsDbId.push(elementDbId);
        }
    }
    this.viewer.isolate(elementsDbId);
    this.viewer.fitToView(elementsDbId);
}

