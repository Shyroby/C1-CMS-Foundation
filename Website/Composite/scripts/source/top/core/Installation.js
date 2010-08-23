/**
 * @class
 */
function _Installation () {
	
	EventBroadcaster.subscribe ( BroadcastMessages.APPLICATION_KICKSTART, this ); 
}

_Installation.prototype = {

    /**
    * Robot readable build version "1.2.3505.18361".
    * @type {string}
    */
    versionString: null,

    /**
    * Human readable product version "Composite C1 1.2 SP2".
    * @type {string}
    */
    versionPrettyString: null,

    /**
    * @type {string}
    */
    installationID: null,

    /**
    * Constructor action: Get installation info.
    * @return {_Installation}
    */
    handleBroadcast: function (broadcast) {

        switch (broadcast) {
            case BroadcastMessages.APPLICATION_KICKSTART:
                var list = new List(InstallationService.GetInstallationInfo(true));
                list.each(function (entry) {
                    switch (entry.Key) {
                        case "ProductVersion":
                            this.versionString = entry.Value;
                            break;
                        case "ProductTitle":
                            this.versionPrettyString = entry.Value;
                            break;
                        case "InstallationId":
                            this.installationID = entry.Value;
                            break;
                    }
                }, this);
                break;
        }
    }
};

/*
 * Here we go.
 */
var Installation = new _Installation ();