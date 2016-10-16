package com.midas.supernavi;

import android.Manifest;
import android.app.DialogFragment;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.RemoteException;
import android.os.Vibrator;
import android.provider.Settings.Secure;
import android.speech.RecognizerIntent;
import android.speech.tts.TextToSpeech;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GestureDetectorCompat;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SeekBar;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.google.gson.Gson;
import com.h6ah4i.android.widget.verticalseekbar.VerticalSeekBar;
import com.midas.supernavi.Models.DtoBeacon;
import com.midas.supernavi.Models.DtoItem;

import org.altbeacon.beacon.Beacon;
import org.altbeacon.beacon.BeaconConsumer;
import org.altbeacon.beacon.BeaconManager;
import org.altbeacon.beacon.BeaconParser;
import org.altbeacon.beacon.RangeNotifier;
import org.altbeacon.beacon.Region;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.List;
import java.util.Locale;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

import static com.midas.supernavi.R.id.groceryList;


public class ProductSelection extends AppCompatActivity implements BeaconConsumer {

    private VerticalSeekBar modeSelector;
    private OperatingMode currentOperatingMode;
    private TextToSpeech textToSpeech;
    private List<String> gList;
    private ArrayAdapter<String> adapter;
    private BeaconManager beaconManager;
    private RequestQueue requestQueue;
    private List<Beacon> currentBeaconList;
    private GestureDetectorCompat mDetector;
    private ScheduledExecutorService executor = Executors.newScheduledThreadPool(1);
    private ScheduledFuture<?> lastFuture;
    private PickUpItemFragment fr;
    private Vibrator vibrator;


    private static final int SPEECH_REQUEST_CODE = 0;

    enum OperatingMode {
        PRODUCT_SELECTION,
        NAVIGATION,
        FREE_ROAM
    }

    class modeListener implements SeekBar.OnSeekBarChangeListener {

        public void onProgressChanged(SeekBar seekBar, int progress,
                                      boolean fromUser) {
            textToSpeech.stop();
            switch (progress) {
                case 0:
                    productSelection();
                    break;
                case 1:
                    navigate();
                    break;
                case 2:
                    freeRoam();
                    break;
                default:
                    throw new IllegalArgumentException("Invalid Mode");
            }
        }

        public void onStartTrackingTouch(SeekBar seekBar) {
        }

        public void onStopTrackingTouch(SeekBar seekBar) {
        }

    }

    public void showPickupDialog() {
        tts("Please tap the screen to confirm you have picked up the item",true);
        if ( fr.getDialog()==null || !fr.getDialog().isShowing()) {
            fr.show(getFragmentManager(), "Pickup");
            vibrate();
        }
    }


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_product_selection);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        populateListView();
        //Default mode is product selection
        setTitle("SuperNavi - Product Selection");
        //currentOperatingMode = OperatingMode.PRODUCT_SELECTION;
        modeSelector = (VerticalSeekBar) findViewById(R.id.modeSelector);
        modeSelector.setOnSeekBarChangeListener(new modeListener());
        Button speakCommand = (Button) findViewById(R.id.speakCommand);
        speakCommand.setOnClickListener(new Button.OnClickListener() {
            public void onClick(View v) {
                textToSpeech.stop();
                displaySpeechRecognizer();
            }
        });


        textToSpeech = new TextToSpeech(getApplicationContext(), new TextToSpeech.OnInitListener() {
            @Override
            public void onInit(int status) {
                if (status != TextToSpeech.ERROR) {
                    textToSpeech.setLanguage(Locale.ENGLISH);
                    textToSpeech.setSpeechRate((float) 0.85);
                    String introMessage = "Welcome to SuperNavi! For instructions on how to use the app, please click on the speak button, which is a large button at the bottom right of the screen. Then say, Getting Started";
                    // textToSpeech.speak(introMessage, TextToSpeech.QUEUE_FLUSH, null, null);
                }
            }
        });
        //textToSpeech.setSpeechRate((float) 0.85);

        int permissionCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.BLUETOOTH);

        int permissionAdminCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.BLUETOOTH_ADMIN);

        int permissionCoarseLocationCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.ACCESS_COARSE_LOCATION);

        int permissionFineLocationCheck = ContextCompat.checkSelfPermission(this,
                Manifest.permission.ACCESS_FINE_LOCATION);

        Log.d("", "Bluetooth: " + permissionAdminCheck);
        Log.d("", "Bluetooth Admin: " + permissionAdminCheck);

        if (permissionCheck != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.BLUETOOTH},
                    0);
        }

        if (permissionAdminCheck != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.BLUETOOTH_ADMIN},
                    0);
        }

        if (permissionCoarseLocationCheck != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.ACCESS_COARSE_LOCATION},
                    0);
        }

        if (permissionFineLocationCheck != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this,
                    new String[]{Manifest.permission.ACCESS_FINE_LOCATION},
                    0);
        }

        beaconManager = BeaconManager.getInstanceForApplication(getApplicationContext());
        beaconManager.getBeaconParsers().add(new BeaconParser().setBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24"));
        beaconManager.setForegroundBetweenScanPeriod(0l);
        beaconManager.setForegroundScanPeriod(5000l);
        beaconManager.bind(this);

        requestQueue = Volley.newRequestQueue(this);
        

        fr = new PickUpItemFragment();

        productSelection();

    }


    public void pickUpItem(View view) {
        if (fr!=null &&  fr.getDialog()!=null
                && fr.getDialog().isShowing()) {
            fr.dismiss();
        }
        sendGetRequest();
    }

    //Handles product selection mode
    private void productSelection() {
        currentOperatingMode = OperatingMode.PRODUCT_SELECTION;
        if (lastFuture != null)
            lastFuture.cancel(true);

        Log.d("Mode", "Entering product selection mode");
        setTitle("SuperNavi - Product Selection");
        tts("Product selection mode", true);


    }

    //Handles navigate mode
    private void navigate() {
        currentOperatingMode = OperatingMode.NAVIGATION;
        Log.d("Mode", "Entering navigation mode");
        setTitle("SuperNavi - Navigation");
        tts("Navigation mode", true);
        List<DtoItem> dtoList = sanitiseList();
        sendPostRequest(dtoList);
        Log.d("Grocery list posted:", dtoList.toString());


    }

    private void sendGetRequest() {
        final String android_id = Secure.getString(this.getApplicationContext().getContentResolver(),
                Secure.ANDROID_ID);
        final Gson gson = new Gson();

        String url = "http://supernavibeaconapi.azurewebsites.net/api/navigation/retrieved/" + android_id;
        StringRequest stringRequest = new StringRequest(Request.Method.GET, url,
                new Response.Listener<String>() {
                    @Override
                    public void onResponse(String response) {
                        tts(response);
                    }
                }, new Response.ErrorListener() {
            @Override
            public void onErrorResponse(VolleyError error) {
            }
        });
        requestQueue.add(stringRequest);
    }

    private void sendPostRequest(List<DtoItem> dtoList) {
        final String android_id = Secure.getString(this.getApplicationContext().getContentResolver(),
                Secure.ANDROID_ID);

        Log.d("AndroidID", android_id);

        if (currentBeaconList.size() == 0) {
            Log.d("", "No beacons found");
            return;
        }

        Log.d("", "" + dtoList.size());

        final Gson gson = new Gson();

        String url = "http://supernavibeaconapi.azurewebsites.net/api/navigation/item/" + android_id;
        try {
            JSONObject jsonObject = new JSONObject();
            jsonObject.put("shoppingList", new JSONArray(gson.toJson(dtoList)));
            jsonObject.put("beacon", new JSONObject(gson.toJson(new DtoBeacon(currentBeaconList.get(0)))));
            JsonObjectRequest jsObjRequest = new JsonObjectRequest
                    (Request.Method.POST, url, jsonObject, new Response.Listener<JSONObject>() {

                        @Override
                        public void onResponse(JSONObject response) {
                            Log.d("hi", "hi");
                            Runnable runnable = new Runnable() {
                                public void run() {
                                    String url1 = "http://supernavibeaconapi.azurewebsites.net/api/navigation/" + android_id;
                                    JSONObject jsonObject1 = new JSONObject();
                                    try {
                                        jsonObject1.put("beacons", new JSONArray(gson.toJson(toDtoBeaconList(currentBeaconList))));
                                        JsonObjectRequest jsonObjectRequest = new JsonObjectRequest
                                                (Request.Method.POST, url1, jsonObject1, new Response.Listener<JSONObject>() {
                                                    @Override
                                                    public void onResponse(JSONObject response) {
                                                        try {
                                                            String responseString = response.getString("str");
                                                            if (responseString.endsWith("SIGNATURE")) {
                                                                responseString = responseString.replaceAll("SIGNATURE", "");
                                                                tts(responseString);
                                                                showPickupDialog();
                                                            } else {
//                                                                if (fr!=null &&  fr.getDialog()!=null
//                                                                        && fr.getDialog().isShowing()){
//                                                                    fr.dismiss();
//                                                                }
                                                                tts(responseString);
                                                            }
                                                        } catch (JSONException e) {
                                                            e.printStackTrace();
                                                        }
                                                        Log.d("Response 2", response.toString());
                                                    }
                                                }, new Response.ErrorListener() {
                                                    @Override
                                                    public void onErrorResponse(VolleyError error) {

                                                    }
                                                });

                                        Log.d("Request 2", jsonObject1.toString());
                                        requestQueue.add(jsonObjectRequest);
                                    } catch (JSONException e) {
                                        e.printStackTrace();
                                    }
                                }
                            };
                            if(lastFuture != null)
                                lastFuture.cancel(true);
                            lastFuture = executor.scheduleAtFixedRate(runnable, 0, 3, TimeUnit.SECONDS);
                            Log.d("Response 1", response.toString());

                        }
                    }, new Response.ErrorListener() {

                        @Override
                        public void onErrorResponse(VolleyError error) {
                            Log.d("", error.toString());
                        }
                    });
            Log.d("Request 1", jsonObject.toString());
            requestQueue.add(jsObjRequest);
        } catch (Exception e) {
            Log.e("Exception:", e.getMessage());
        }
    }

    private List<DtoBeacon> toDtoBeaconList(List<Beacon> currentBeaconList) {
        List<DtoBeacon> list = new ArrayList<DtoBeacon>();

        for (Beacon beacon : currentBeaconList) {
            DtoBeacon dto = new DtoBeacon(beacon);
            list.add(dto);
        }

        return list;
    }

    private List<DtoItem> sanitiseList() {
        ArrayList<DtoItem> dtoList = new ArrayList<DtoItem>();
        for (String item : gList) {
            DtoItem dtoItem = new DtoItem(item);
            dtoList.add(dtoItem);
        }
        return dtoList;
    }


    //Handles free roam mode
    private void freeRoam() {
        currentOperatingMode = OperatingMode.FREE_ROAM;
        if (lastFuture != null)
            lastFuture.cancel(true);
        Log.d("Mode", "Entering free roam mode");
        setTitle("SuperNavi - Free Roam");
        tts("free roam mode", true);


    }

    // Show google speech recognizer
    private void displaySpeechRecognizer() {
        Intent intent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
        intent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL,
                RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
        // Start the activity, the intent will be populated with the speech text
        startActivityForResult(intent, SPEECH_REQUEST_CODE);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode,
                                    Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == SPEECH_REQUEST_CODE && resultCode == RESULT_OK) {
            ArrayList<String> matches = data.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
            Log.d("Matches:", matches.toString());
            //Global commands to change mode etc
            checkCommand(matches);
        } else {
            tts("Sorry, I could not recognise that last command, please try again", true);
        }
    }

    private boolean checkCommand(ArrayList<String> matches) {

        String[] addOrDelete = matches.get(0).split(" ");

        //Global commands
        if (matches.contains("product selection mode") || matches.contains("product selection") || matches.contains("selection") || matches.contains("select") || matches.contains("product")) {
            if (currentOperatingMode == OperatingMode.PRODUCT_SELECTION) {
                tts("You are already in product selection mode!");
            }
            modeSelector.setProgress(0);
            return true;
        } else if (matches.contains("navigate") || matches.contains("navigation") || matches.contains("navigation mode")) {
            if (currentOperatingMode == OperatingMode.NAVIGATION) {
                tts("You are already in navigation mode!");
            }
            modeSelector.setProgress(1);
            return true;
        } else if (matches.contains("free roam mode") || matches.contains("free roam") || matches.contains("roam")) {
            if (currentOperatingMode == OperatingMode.FREE_ROAM) {
                tts("You are already in free roam mode!");
            }
            modeSelector.setProgress(2);
            return true;
        } else if (matches.contains("exit") || matches.contains("quit") || matches.contains("finished")) {
            tts("Exiting app");
            finish();
            return true;
        } else if (matches.contains("help")) {
            help(matches);
            return true;
        } else if (matches.contains("read shopping list")) {
            readShoppingList();
            return true;
        } else if (matches.contains("clear shopping list") || matches.contains("reset shopping list") || matches.contains("empty shopping list")) {
            clearShoppingList();
        } else if (matches.contains("getting started")) {
            tts("There is a mode slider along the left edge of the screen. Drag this to change modes. The large speak command button is at the bottom right of the screen, click on this and say a command. Say help to get info on these commands for each mode", true);
        } else if (currentOperatingMode == OperatingMode.PRODUCT_SELECTION) {
            if (addOrDelete[0].equals("ad") || addOrDelete[0].equals("add")) {
                addItem(addOrDelete);
            } else if (addOrDelete[0].equals("remove") || addOrDelete[0].equals("delete")) {
                deleteItem(addOrDelete);
            } else {
                tts("Sorry, I could not recognise that last command, please try again", true);
            }
        } else if (currentOperatingMode != OperatingMode.PRODUCT_SELECTION && addOrDelete[0].equals("ad") || addOrDelete[0].equals("add") || addOrDelete[0].equals("remove") || addOrDelete[0].equals("delete")) {
            tts("Cannot add or delete in this mode, please change to product selection mode", true);
        } else if (currentOperatingMode == OperatingMode.FREE_ROAM && matches.contains("whats here")) {
            freeRoamQuery();
        } else {
            tts("Sorry, I could not recognise that last command, please try again", true);
        }

        return false;
    }

    private void freeRoamQuery(){
        final String android_id = Secure.getString(this.getApplicationContext().getContentResolver(),
                Secure.ANDROID_ID);

        if (currentBeaconList.size() == 0) {
            return;
        }
        final Gson gson = new Gson();

        String url = "http://supernavibeaconapi.azurewebsites.net/api/navigation/item/" + android_id;
        JSONObject jsonObject1 = new JSONObject();
        try {
            jsonObject1.put("beacons", new JSONArray(gson.toJson(toDtoBeaconList(currentBeaconList))));
            JsonObjectRequest jsonObjectRequest = new JsonObjectRequest
                    (Request.Method.POST, url, jsonObject1, new Response.Listener<JSONObject>() {
                        @Override
                        public void onResponse(JSONObject response) {
                            try {
                                String responseString = response.getString("str");
                                tts(responseString);
                            } catch (JSONException e) {
                                e.printStackTrace();
                            }
                            Log.d("Response 2", response.toString());
                        }
                    }, new Response.ErrorListener() {
                        @Override
                        public void onErrorResponse(VolleyError error) {

                        }
                    });
            requestQueue.add(jsonObjectRequest);
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }

    private void clearShoppingList() {
        for (int i = gList.size() - 1; i >= 0; i--) {
            gList.remove(i);
        }
        adapter.notifyDataSetChanged();
        tts("Shopping list cleared!");
    }

    private void readShoppingList() {
        if (gList.size() == 0) {
            tts("Your shopping list is empty! Please add some items");
        } else {
            tts("Your shopping list contains:");
            Log.v("Grocery list:", gList.toString());
            for (String grocery : gList) {
                Log.d("grocery:", grocery);
                tts(grocery);
            }
        }

    }

    private void help(ArrayList<String> matches) {
        switch (currentOperatingMode) {
            case PRODUCT_SELECTION:
                tts("You are in product selection mode. Simply click the speak button and say add and then the item you want to add to your shopping list", true);
                break;
            case FREE_ROAM:
                tts("You are in free roam mode. I will say what items you are passing in this supermarket as you walk around freely", true);
                break;
            case NAVIGATION:
                tts("You are in navigation mode. I will guide you around to pick up all the items on your grocery list", true);
                break;
            default:
                throw new IllegalArgumentException("Invalid Mode");
        }

    }

    private void tts(final String toSpeak) {
        Thread t1 = new Thread() {

            @Override
            public void run() {
                textToSpeech.speak(toSpeak, TextToSpeech.QUEUE_FLUSH, null, null);
                while (textToSpeech.isSpeaking()) {
                }
                try {
                    Thread.sleep(300);
                } catch (
                        InterruptedException e
                        ) {
                    Log.e("Exception:", e.getMessage());
                }

            }

            ;
        };
        t1.start();
        try {
            t1.join();
        } catch (InterruptedException e) {
        }
    }


    //Interruptible tts overloaded method
    private void tts(String toSpeak, boolean interruptible) {
        textToSpeech.speak(toSpeak, TextToSpeech.QUEUE_FLUSH, null, null);
    }


    //Add items to shopping list
    private void addItem(String[] groceryStrArray) {
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 1; i < groceryStrArray.length; i++) {
            strBuilder.append(groceryStrArray[i] + " ");
        }
        String grocery = strBuilder.toString().trim();
        if (!gList.contains(grocery)) {
            gList.add(grocery);
            adapter.notifyDataSetChanged();
            tts("Added " + grocery + " to shopping list");
        } else {
            tts("Your shopping list already contains " + grocery);
        }
    }

    //Delete items from shopping list
    private void deleteItem(String[] groceryStrArray) {
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 1; i < groceryStrArray.length; i++) {
            strBuilder.append(groceryStrArray[i] + " ");
        }
        String grocery = strBuilder.toString().trim();
        Log.d("Deleted:", grocery);
        if (gList.contains(grocery)) {
            gList.remove(grocery);
            adapter.notifyDataSetChanged();
            tts("Removed " + grocery + " from shopping list");
        } else {
            tts("Your shopping list does not contain " + grocery);
        }
    }

    private void vibrate(){
        vibrator = (Vibrator) this.getApplicationContext().getSystemService(Context.VIBRATOR_SERVICE);
        //Vibrate for 500ms
        vibrator.vibrate(500);
    }


    //Creates grocery list
    private void populateListView() {
        String[] groceries = {"banana","yoghurt"};
        gList = new ArrayList<String>(Arrays.asList(groceries));

        adapter = new ArrayAdapter<String>(this, R.layout.groceries, gList);
        ListView groceryListView = (ListView) findViewById(groceryList);
        groceryListView.setAdapter(adapter);
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_product_selection, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    public void onBeaconServiceConnect() {
        beaconManager.setRangeNotifier(new RangeNotifier() {
            @Override
            public void didRangeBeaconsInRegion(Collection<Beacon> collection, Region region) {
                Log.d("", "Count: " + collection.size());

                currentBeaconList = new ArrayList<>(collection);
            }
        });

        try {
            beaconManager.startRangingBeaconsInRegion(new Region("defaultRegion", null, null, null));
        } catch (RemoteException e) {

        }
    }


    public static class PickUpItemFragment extends DialogFragment {
        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                                 Bundle savedInstanceState) {
            View view = inflater.inflate(R.layout.pickup_dialog, container);
            getDialog().setTitle("Hello");

            return view;
        }

    }


}

