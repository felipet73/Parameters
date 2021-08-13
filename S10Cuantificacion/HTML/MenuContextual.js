class MenuContextual extends Autodesk.Viewing.Extension {
    constructor(viewer, options) {
        super(viewer, options);
        this.onCreacionContextualMenuItem = this.onCreacionContextualMenuItem.bind(this);
    }

    get menuId() {
        return 'ItemMenuContextual';
    }
     
     onCreacionContextualMenuItem(menu, status) {
        if (status.hasSelected) {
            if (viewer.getSelection().length > 0) {
                menu.push({
                    title: 'Obtener datos seleccion',
                    target: () => {
                        var uniqueIds = [];
                        var DBids = viewer.getSelection();
                        var n = 0;
                        for (var uniqueId of DBids) {
                            var objSelected = viewer.getSelection()[n];
                            n = n + 1;
                            this.viewer.getProperties(objSelected, (props) => {
                                uniqueIds.push(props.externalId);
                                //alert(uniqueIds);
                                if (n == DBids.length) {
                                    callbackObj.showMessage(uniqueIds, DBids.length);
                                    //callbackObj.returnex(uniqueIds);
                                    //alert(uniqueIds);
                                    console.log(uniqueIds);
                                    //9c9538fd-af40-4b3d-bd89-f8e4acac1fd8-000525ae
                                }
                            });



    
                        }
                        
                        //alert(uniqueIds);
                        //callbackObj.showMessage(uniqueIds);


                        //await DBids.forEach( (objSelected) => {
                        //      //   var objSelected = viewer.getSelection()[uniqueId];
                        //      this.viewer.getProperties(objSelected, (props) => {
                        //          uniqueIds.push(props.externalId);
                        //      });

                        //  });
                        // callbackObj.showMessage(uniqueIds);
                    }
                });
            }//cierro if getSelection()=1    

        }// cierro (status.hasSelected

    }

    load() {
        // Creación menu contextual item
        this.viewer.registerContextMenuCallback(
            this.menuId,
            this.onCreacionContextualMenuItem
        );
        return true;
    }

    unload() {
        // Borrado de todas los items
        this.viewer.unregisterContextMenuCallback(this.menuId);
        return true;
    }

}
Autodesk.Viewing.theExtensionManager.registerExtension('MenuContextual', MenuContextual);

 function addIds(DBids, uniqueIds, callback) {
    var n = 0;
    for (var uniqueId of DBids) {
        var objSelected = viewer.getSelection()[uniqueId];
        this.viewer.getProperties(objSelected, (props) => {
            uniqueIds.push(props.externalId);
            n = n++;
            if (n == DBids.length) {
                callback(null, uniqueIds)
            }
        });  
       
    }
   
}