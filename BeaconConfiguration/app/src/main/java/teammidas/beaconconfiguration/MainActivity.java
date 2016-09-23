package teammidas.beaconconfiguration;

import android.os.RemoteException;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.android.volley.AuthFailureError;
import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;

import org.altbeacon.beacon.Beacon;
import org.altbeacon.beacon.BeaconConsumer;
import org.altbeacon.beacon.BeaconManager;
import org.altbeacon.beacon.BeaconParser;
import org.altbeacon.beacon.RangeNotifier;
import org.altbeacon.beacon.Region;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

public class MainActivity extends AppCompatActivity implements BeaconConsumer {

    private static final String TAG = "MainActivity";

    private Button sendButton;
    private EditText supermarketTextField;
    private EditText xCoordinateTextField;
    private EditText yCoordinateTextField;
    private BeaconManager beaconManager;
    private List<Beacon> currentBeaconList;

    private final String URL = "http://www.azuresomething.com";
    private RequestQueue requestQueue = Volley.newRequestQueue(this);

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        sendButton = (Button) findViewById(R.id.button);
        supermarketTextField = (EditText) findViewById(R.id.editText4);
        xCoordinateTextField = (EditText) findViewById(R.id.editText3);
        yCoordinateTextField = (EditText) findViewById(R.id.editText2);

        sendButton.setOnClickListener(buttonHandler);

        beaconManager = BeaconManager.getInstanceForApplication(getApplicationContext());
        beaconManager.getBeaconParsers().add(new BeaconParser().
                setBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));
        beaconManager.setForegroundBetweenScanPeriod(2000l);
        beaconManager.setBackgroundScanPeriod(5000l);
        beaconManager.bind(this);
    }

    View.OnClickListener buttonHandler = new View.OnClickListener(){

        @Override
        public void onClick(View v) {
            Log.d(TAG, "Supermarket: " + supermarketTextField.getText().toString() +
                       " x: " + xCoordinateTextField.getText().toString() +
                       " y: " + yCoordinateTextField.getText().toString());

            StringRequest stringRequest = new StringRequest(Request.Method.POST, URL, new Response.Listener<String>() {
                @Override
                public void onResponse(String response) {

                }
            }, new Response.ErrorListener() {
                @Override
                public void onErrorResponse(VolleyError error) {

                }
            }){
                @Override
                protected Map<String, String> getParams(){
                    Map<String, String> params = new HashMap<>();

                    return params;
                }

                @Override
                public Map<String, String> getHeaders() throws AuthFailureError{
                    Map<String, String> params = new HashMap<>();
                    params.put("Content-type", "application/x-www-form-urlencoded");
                    return params;
                }
            };

            requestQueue.add(stringRequest);
        }
    };


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

    @Override
    public void onDestroy(){
        super.onDestroy();
        beaconManager.unbind(this);
    }
}
