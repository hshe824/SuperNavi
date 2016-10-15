package com.midas.supernavi.Models;

import org.altbeacon.beacon.Beacon;

/**
 * Created by hshe on 15/10/2016.
 */

public class DtoBeacon {

    public DtoBeacon(Beacon beacon){
        this.uuid = beacon.getId1().toString();
        this.majorid = beacon.getId2().toInt();
        this.minorid = beacon.getId3().toInt();
        this.rssi = beacon.getRssi();
        this.positionX = 0;
        this.positionY = 0;
    }

    public String uuid;
    public int majorid;
    public int minorid;
    public int rssi;
    public String supermarket;
    public int positionX;
    public int positionY;
    public int count = 0;
}
