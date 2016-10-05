package com.midas.supernavi;

import android.app.Service;
import android.content.Intent;
import android.graphics.Region;
import android.os.IBinder;
import android.os.RemoteException;
import android.util.Log;

import org.altbeacon.beacon.BeaconConsumer;

import java.util.ArrayList;


public class NavigationService extends Service implements BeaconConsumer  {

    private OperatingMode currentOperatingMode;

    enum OperatingMode {
        PRODUCT_SELECTION,
        FREE_ROAM,
        NAVIGATION
    }



    public NavigationService() {
    }

    @Override
    public IBinder onBind(Intent intent) {
        // TODO: Return the communication channel to the service.
    }


    @Override
    public void onBeaconServiceConnect() {
        beaconManager.addRangeNotifier(new RangeNotifier() {
            @Override
            public void didRangeBeaconsInRegion(Collection<Beacon> collection, Region region) {
                Iterator<Beacon> iterator = collection.iterator();
                while(iterator.hasNext()){
                    Beacon beacon = iterator.next();
                    Log.d(TAG, "Beacon ID1: " + beacon.getId1());
                    Log.d(TAG, "Beacon ID2: " + beacon.getId2());
                    Log.d(TAG, "Beacon ID3: " + beacon.getId3());
                    Log.d(TAG, "Beacon ID3: " + beacon.getRssi());
                }
                currentBeaconList = new ArrayList<>(collection);
            }
        });

        try{
            beaconManager.startRangingBeaconsInRegion(new Region("defaultRegion", null, null, null));
        }catch(RemoteException e){

        }
    }
}
