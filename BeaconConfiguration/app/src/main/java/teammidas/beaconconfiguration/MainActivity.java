package teammidas.beaconconfiguration;

import android.Manifest;
import android.content.Context;
import android.content.pm.PackageManager;
import android.os.RemoteException;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import org.altbeacon.beacon.Beacon;
import org.altbeacon.beacon.BeaconConsumer;
import org.altbeacon.beacon.BeaconManager;
import org.altbeacon.beacon.BeaconParser;
import org.altbeacon.beacon.RangeNotifier;
import org.altbeacon.beacon.Region;
import org.json.JSONException;
import org.json.JSONObject;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Iterator;
import java.util.List;

public class
MainActivity extends AppCompatActivity implements BeaconConsumer {

    private static final String TAG = "MainActivity";

    private Button sendButton;
    private EditText supermarketTextField;
    private EditText xCoordinateTextField;
    private EditText yCoordinateTextField;
    private BeaconManager beaconManager;
    private List<Beacon> currentBeaconList;

    private final String URL = "http://supernavibeaconapi.azurewebsites.net/api/Beacon";
    // final String URL = "http://localhost:43865/api/Beacon";
    private RequestQueue requestQueue;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        requestQueue = Volley.newRequestQueue(this);
        sendButton = (Button) findViewById(R.id.button);
        supermarketTextField = (EditText) findViewById(R.id.editText4);
        xCoordinateTextField = (EditText) findViewById(R.id.editText3);
        yCoordinateTextField = (EditText) findViewById(R.id.editText2);

        sendButton.setOnClickListener(buttonHandler);

        beaconManager = BeaconManager.getInstanceForApplication(getApplicationContext());
        beaconManager.getBeaconParsers().add(new BeaconParser().setBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));
        beaconManager.setForegroundBetweenScanPeriod(0l);
        beaconManager.setForegroundScanPeriod(5000l);
        beaconManager.bind(this);


        int permissionCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.BLUETOOTH);

        int permissionAdminCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.BLUETOOTH_ADMIN);

        int permissionCoarseLocationCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.ACCESS_COARSE_LOCATION);

        int permissionFineLocationCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.ACCESS_FINE_LOCATION);

        Log.d(TAG, "Bluetooth: " + permissionAdminCheck);
        Log.d(TAG, "Bluetooth Admin: " + permissionAdminCheck);

        if(permissionCheck != PackageManager.PERMISSION_GRANTED){
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.BLUETOOTH},
                    0);
        }

        if(permissionAdminCheck != PackageManager.PERMISSION_GRANTED){
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.BLUETOOTH_ADMIN},
                    0);
        }

        if(permissionCoarseLocationCheck != PackageManager.PERMISSION_GRANTED){
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.ACCESS_COARSE_LOCATION},
                    0);
        }

        if(permissionFineLocationCheck != PackageManager.PERMISSION_GRANTED){
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.ACCESS_FINE_LOCATION},
                    0);
        }


    }

    View.OnClickListener buttonHandler = new View.OnClickListener(){

        @Override
        public void onClick(View v) {
            Log.d(TAG, "Supermarket: " + supermarketTextField.getText().toString() +
                       " x: " + xCoordinateTextField.getText().toString() +
                       " y: " + yCoordinateTextField.getText().toString());

            for(Beacon beacon : currentBeaconList){

                JSONObject jsonObject = new JSONObject();
                try {
                    jsonObject.put("uuid", beacon.getId1().toString());
                    jsonObject.put("majorid", beacon.getId2().toInt());
                    jsonObject.put("minorid", beacon.getId3().toInt());
                    jsonObject.put("positionX", Integer.parseInt(xCoordinateTextField.getText().toString()));
                    jsonObject.put("positionY", Integer.parseInt(yCoordinateTextField.getText().toString()));
                    jsonObject.put("rssi", beacon.getRssi());
                    jsonObject.put("supermarket", supermarketTextField.getText().toString());
                    jsonObject.put("count", 1);

                } catch (JSONException e) {
                    e.printStackTrace();
                }

                JsonObjectRequest jsonRequest = new JsonObjectRequest(Request.Method.POST, URL, jsonObject, new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject jsonObject) {
                        Log.e(TAG, "Response " + jsonObject.toString());
                    }
                }, new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError volleyError) {

                    }
                });
                Log.d(TAG, jsonObject.toString());
                requestQueue.add(jsonRequest);
            }

        }
    };


    @Override
    public void onBeaconServiceConnect() {
        beaconManager.setRangeNotifier(new RangeNotifier() {
            @Override
            public void didRangeBeaconsInRegion(Collection<Beacon> collection, Region region) {
                Log.d(TAG, "Count: " + collection.size());

                currentBeaconList = new ArrayList<>(collection);
            }
        });

        try{
            beaconManager.startRangingBeaconsInRegion(new Region("defaultRegion", null, null, null));
        }catch(RemoteException e){

        }
    }

    @Override
    public void onDestroy(){
        super.onDestroy();
        beaconManager.unbind(this);
    }
}
