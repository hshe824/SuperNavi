package com.midas.supernavi;

import android.content.Intent;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.speech.tts.TextToSpeech;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.SeekBar;

import com.h6ah4i.android.widget.verticalseekbar.VerticalSeekBar;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Locale;

import static com.midas.supernavi.R.id.groceryList;
import static com.midas.supernavi.R.id.text;

public class ProductSelection extends AppCompatActivity {

    private VerticalSeekBar modeSelector;
    private OperatingMode currentOperatingMode;
    private TextToSpeech textToSpeech;
    private List<String> gList;
    private ArrayAdapter<String> adapter;

    private static final int SPEECH_REQUEST_CODE = 0;


    enum OperatingMode {
        PRODUCT_SELECTION,
        NAVIGATION,
        FREE_ROAM
    }

    class modeListener implements SeekBar.OnSeekBarChangeListener {

        public void onProgressChanged(SeekBar seekBar, int progress,
                                      boolean fromUser) {
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

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_product_selection);
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        populateListView();
        //Default mode is product selection
        setTitle("SuperNavi - Product Selection");
        currentOperatingMode = OperatingMode.PRODUCT_SELECTION;
        modeSelector = (VerticalSeekBar) findViewById(R.id.modeSelector);
        modeSelector.setOnSeekBarChangeListener(new modeListener());
        Button speakCommand = (Button) findViewById(R.id.speakCommand);
        speakCommand.setOnClickListener(new Button.OnClickListener() {
            public void onClick(View v) {
                displaySpeechRecognizer();
            }
        });

        textToSpeech = new TextToSpeech(getApplicationContext(), new TextToSpeech.OnInitListener() {
            @Override
            public void onInit(int status) {
                if (status != TextToSpeech.ERROR) {
                    textToSpeech.setLanguage(Locale.ENGLISH);
                }
            }
        });
        textToSpeech.setSpeechRate((float) 0.85);
        textToSpeech.speak("Press speak button on the bottom of the screen and say an item to add it to the shopping list", TextToSpeech.QUEUE_FLUSH, null, null);


    }

    //Handles product selection mode
    private void productSelection() {
        currentOperatingMode = OperatingMode.PRODUCT_SELECTION;
        Log.d("Mode", "Entering product selection mode");
        setTitle("SuperNavi - Product Selection");
        tts("Entering product selection mode");


    }

    //Handles navigate mode
    private void navigate() {
        currentOperatingMode = OperatingMode.NAVIGATION;
        Log.d("Mode", "Entering navigation mode");
        setTitle("SuperNavi - Navigation");
        tts("Entering navigation mode");


    }


    //Handles free roam mode
    private void freeRoam() {
        currentOperatingMode = OperatingMode.FREE_ROAM;
        Log.d("Mode", "Entering free roam mode");
        setTitle("SuperNavi - Free Roam");
        tts("Entering free roam mode");


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
        } else if (matches.contains("where am i")) {
            //TODO: Tell user where they are
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
        } else if (currentOperatingMode == OperatingMode.PRODUCT_SELECTION && (addOrDelete[0].equals("ad") || addOrDelete[0].equals("add"))){
            addItem(addOrDelete);
        } else if (currentOperatingMode == OperatingMode.PRODUCT_SELECTION && addOrDelete[0].equals("remove") || addOrDelete[0].equals("delete")){
            deleteItem(addOrDelete);
        } else {
            tts("Sorry, I could not recognise that last command, please try again");
        }

        return false;
    }

    private void readShoppingList() {
        tts("Your shopping list contains:");
        Log.v("Grocery list:", gList.toString());
        for (String grocery : gList) {
            Log.d("grocery:", grocery);
            tts(grocery);
        }

    }

    private void help(ArrayList<String> matches) {
        switch (currentOperatingMode) {
            case PRODUCT_SELECTION:
                tts("You are in product selection mode. Simply click the speak button and say an item you want to add to your shopping list");
                break;
            case FREE_ROAM:
                tts("You are in free roam mode. I will say what items you are passing in this supermarket as you walk around freely");
                break;
            case NAVIGATION:
                tts("You are in navigation mode. I will guide you around to pick up all the items on your grocery list");
                break;
            default:
                throw new IllegalArgumentException("Invalid Mode");
        }

    }

    private void tts(String toSpeak) {
        textToSpeech.speak(toSpeak, TextToSpeech.QUEUE_FLUSH, null, null);
        while (textToSpeech.isSpeaking()) {

        }
        try {
            Thread.sleep(300);
        } catch (InterruptedException e){
            Log.e("Exception:",e.getMessage());
        }
    }

    //Add items to shopping list
    private void addItem(String[] groceryStrArray) {
        StringBuilder strBuilder = new StringBuilder();
        for (int i = 1; i < groceryStrArray.length; i++) {
            strBuilder.append(groceryStrArray[i]+" ");
        }
        String grocery = strBuilder.toString();
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
            strBuilder.append(groceryStrArray[i]+" ");
        }
        String grocery = strBuilder.toString();
        if (gList.contains(grocery)) {
            gList.remove(grocery);
            adapter.notifyDataSetChanged();
            tts("Removed " + grocery + " from shopping list");
        } else {
            tts("Your shopping list does not contains " + grocery);
        }
    }


    //Creates grocery list
    private void populateListView() {
        String[] groceries = {"bananas", "milk", "steak", "lettuce", "chips", "bread"};
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
}
