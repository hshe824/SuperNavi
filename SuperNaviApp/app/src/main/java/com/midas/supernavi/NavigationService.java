package com.midas.supernavi;

import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.os.IBinder;
import android.os.PowerManager;
import android.os.RemoteException;
import android.os.Vibrator;
import android.util.Log;

import org.altbeacon.beacon.Beacon;
import org.altbeacon.beacon.BeaconConsumer;
import org.altbeacon.beacon.BeaconManager;
import org.altbeacon.beacon.RangeNotifier;
import org.altbeacon.beacon.Region;

import java.util.Collection;
import java.util.Iterator;


public class NavigationService extends Service implements BeaconConsumer  {

    private static final String TAG = "NavigationService";

    //State fields
    private Context context;
    private BeaconManager beaconManager;

    //Service objects
    private PowerManager.WakeLock wakeLock;
    private Vibrator vibrator;


    //Free roam:
    // You are in aisle x, this has a,b, c
    // You are passing bananas


    @Override
    public void onCreate() {
        super.onCreate();
        PowerManager powerManager = (PowerManager) getSystemService(this.POWER_SERVICE);
        wakeLock = powerManager.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "DoNotSleep");
        context = this.getApplicationContext();
        Log.i("Navigation Service", "        Service Created");

    }
    private void vibrate(){
        vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        //Vibrate for 500ms
        vibrator.vibrate(500);
    }


    public NavigationService() {
    }


    @Override
    public IBinder onBind(Intent intent) {
        // TODO: Return the communication channel to the service.
        return null;
    }


    @Override
    public void onBeaconServiceConnect() {

        beaconManager.setRangeNotifier(new RangeNotifier() {
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
            }
        });

        try{
            beaconManager.startRangingBeaconsInRegion(new Region("defaultRegion", null, null, null));
        }catch(RemoteException e){

        }
    }
}
