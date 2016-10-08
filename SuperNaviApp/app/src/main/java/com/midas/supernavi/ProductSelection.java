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

public class ProductSelection extends AppCompatActivity {

    private VerticalSeekBar modeSelector;
    private OperatingMode currentOperatingMode;
    private TextToSpeech textToSpeech;

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

    //Handles product selection mode
    private void productSelection() {
        currentOperatingMode = OperatingMode.PRODUCT_SELECTION;
        Log.d("Mode","Entering product selection mode");
        setTitle("SuperNavi - Product Selection");
        textToSpeech.speak("Entering product selection mode",TextToSpeech.QUEUE_FLUSH, null,null);



    }

    //Handles navigate mode
    private void navigate() {
        currentOperatingMode = OperatingMode.NAVIGATION;
        Log.d("Mode","Entering navigation mode");
        setTitle("SuperNavi - Navigate");
        textToSpeech.speak("Entering navigation mode",TextToSpeech.QUEUE_FLUSH, null,null);


    }


    //Handles free roam mode
    private void freeRoam() {
        currentOperatingMode = OperatingMode.FREE_ROAM;
        Log.d("Mode","Entering free roam mode");
        setTitle("SuperNavi - FreeRoam");
        textToSpeech.speak("Entering free roam mode",TextToSpeech.QUEUE_FLUSH, null,null);



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
            Log.d("Matches:",matches.toString());
            //Global commands to change mode etc
            if (!checkCommand(matches)){
                //
            }


        }
    }

    private boolean checkCommand(ArrayList<String> matches){
        if (matches.contains("product selection") ||matches.contains("selection") || matches.contains("select") || matches.contains("product")) {
            modeSelector.setProgress(0);
            return true;
        } else if (matches.contains("navigate") || matches.contains("navigation")) {
            modeSelector.setProgress(1);
            return true;
        } else if (matches.contains("free roam") || matches.contains("free") || matches.contains("roam")) {
            modeSelector.setProgress(2);
            return true;
        } else if (matches.contains("where") || matches.contains("am") || matches.contains("i")){
            //TODO: Tell user where they are
            return true;
        }
        return false;
    }


        @Override
        protected void onCreate (Bundle savedInstanceState){
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

            textToSpeech=new TextToSpeech(getApplicationContext(), new TextToSpeech.OnInitListener() {
                @Override
                public void onInit(int status) {
                    if(status != TextToSpeech.ERROR) {
                        textToSpeech.setLanguage(Locale.ENGLISH);
                    }
                }
            });
            textToSpeech.setSpeechRate((float)0.75);

        }

        //Creates grocery list

    private void populateListView() {
        String[] groceries = {"Bananas", "Milk", "Steak", "Lettuce", "Chips", "Bread"};
        final List<String> groceryList = new ArrayList<String>(Arrays.asList(groceries));
        ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, R.layout.groceries, groceryList);
        ListView list = (ListView) findViewById(R.id.groceryList);
        list.setAdapter(adapter);
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
