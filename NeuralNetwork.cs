using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NeuralNetwork : MonoBehaviour
{

    [SerializeField] private int NumInputLayers;
    [SerializeField] private int NumOutputLayers;
    [SerializeField] private bool ShowWeights = false;

    [SerializeField] private int NumHiddenLayers;
    [SerializeField] private int NumNeurons;

    private List<float[,]> weights = new List<float[,]>();
    private List<float[,]> biases = new List<float[,]>();
    private List<float[]> neurons = new List<float[]>();
    private List<float> outputLayer = new List<float>();
    private List<float> inputLayer = new List<float>();


    
    private void Awake()
    {
        clicked = new bool[NumInputLayers];
        Initalise();

        RandomizeWeights();
        print(NumNeurons);
        RandomizeBiases();
    }


    void RunNetwork()
    {
        if (once)
        {
            //Calculate Input Layer to first Neuron
            for (int i = 0; i < NumNeurons; i++)
            {
                for (int j = 0; j < NumInputLayers; j++)
                {
                    neurons[0][i] += inputLayer[j] * weights[0][j, i] + biases[0][j, i];
                    print(inputLayer[j]);
                }
                neurons[0][i] = SoftPlus(neurons[0][i]);
            }
            //Calculate hiddenLayers
            for (int i = 1; i < NumHiddenLayers; i++)
            {
                for (int j = 0; j < NumNeurons; j++)
                {
                    for (int f = 0; f < NumNeurons; f++)
                    {
                        neurons[i][j] += neurons[i - 1][j] * weights[i][f, j] + biases[i][f, j];

                    }
                    neurons[i][j] = (float)System.Math.Tanh(neurons[i][j]);
                }
            }
            //Calculate Output Layer
            for (int i = 0; i < NumOutputLayers; i++)
            {
                for (int j = 0; j < weights[NumHiddenLayers].GetLength(0); j++)
                {
                    outputLayer[i] += neurons[NumHiddenLayers - 1][j] * weights[NumHiddenLayers][j, i] + biases[NumHiddenLayers][j, i];
                }
                outputLayer[i] = Sigmoid(outputLayer[i]);
            }
            once = false;
        }
    }
    private void Initalise()
    {
        float[] counter;
        float[,] WCounter = new float[NumInputLayers, NumNeurons];
        float[,] BCounter = new float[NumInputLayers, NumNeurons];
        weights.Add(WCounter);
        biases.Add(BCounter);

        WCounter = new float[NumNeurons, NumNeurons];
        BCounter = new float[NumNeurons, NumNeurons];
        for (int i = 0; i < NumHiddenLayers - 1; i++)
        {
            weights.Add(WCounter);
            biases.Add(BCounter);
        }
        WCounter = new float[NumNeurons, NumOutputLayers];
        weights.Add(WCounter);
        biases.Add(BCounter);


        print("Weights count: " + weights.Count);
        for (int i = 0; i < NumHiddenLayers; i++)
        {
            counter = new float[NumNeurons];
            neurons.Add(counter);
        }
        for (int i = 0; i < NumInputLayers; i++)
        {
            inputLayer.Add(0);
        }
        for (int i = 0; i < NumOutputLayers; i++)
        {
            outputLayer.Add(0);
        }
    }

    void RandomizeWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            for (int j = 0; j < weights[i].GetLength(0); j++)
            {
                for (int f = 0; f < weights[i].GetLength(1); f++)
                {
                    weights[i][j, f] = Random.Range(-1f, 1f);
                    //print("Weights:" + weights[i][j, f]);
                }
            }
        }
    }
    void RandomizeBiases()
    {
        for (int i = 0; i < biases.Count; i++)
        {
            for (int j = 0; j < biases[i].GetLength(0); j++)
            {
                for (int f = 0; f < biases[i].GetLength(1); f++)
                {
                    biases[i][j, f] = Random.Range(-1f, 1f);
                    //print("Biases:" + biases[i][j, f]);
                }
            }
        }
    }
    private void OnGUI()
    {
        Visualize();

    }
    bool[] clicked;
    bool Clicked = false;
    bool once = false;
    bool onceCalled = false;
    void Visualize()
    {
        if (GUI.Button(new Rect(100, 100, 90, 40), "CALCULATE"))
        {
            Clicked = !Clicked;          
        }
        if (Clicked && onceCalled == false)
        {
            once = true;
            onceCalled = true;
            RunNetwork();
        }
        else if(!Clicked && onceCalled)
        {
            ClearNetwork();
            onceCalled = false;
        }

        for (int i = 0; i < NumInputLayers; i++)
        {

            if (GUI.Button(new Rect(300, Screen.height / 2 - ((NumInputLayers - 1) * 80 / 2) + 80 * i, 40, 40), inputLayer[i].ToString()))
            {
                clicked[i] = !clicked[i];
            }
            if (clicked[i])
                inputLayer[i] = 1;
            else
                inputLayer[i] = 0;
        }
        for (int j = 0; j < NumHiddenLayers; j++)
        {
            for (int i = 0; i < NumNeurons; i++)
            {
                GUI.Box(new Rect(500 + j * 260, Screen.height / 2 - ((NumNeurons - 1) * 80 / 2) + 80 * i, 100, 40), neurons[j][i].ToString());
            }
        }
        for (int i = 0; i < NumOutputLayers; i++)
        {
            GUI.Box(new Rect(400 + NumHiddenLayers * 260 + 100, Screen.height / 2 - ((NumOutputLayers - 1) * 80 / 2) + 80 * i, 100, 40), outputLayer[i].ToString());
        }


        //show weights an biases

        //value = EditorGUI.Slider(new Rect(100, 500, 150, 20), value, -1f, 1f);
        if (ShowWeights)
        {
            for (int i = 0; i < weights.Count; i++)
            {
                for (int j = 0; j < weights[i].GetLength(0); j++)
                {
                    for (int f = 0; f < weights[i].GetLength(1); f++)
                    {
                        if (weights.Count != NumHiddenLayers - 1)
                            weights[i][j, f] = EditorGUI.Slider(new Rect(345 + i * 260, Screen.height / 2 - ((20 * weights[i].GetLength(0) * weights[i].GetLength(1)) / 2) + (20 * j * NumNeurons) + 20 * f, 150, 20), weights[i][j, f], -1f, 1f);
                        else
                            weights[i][j, f] = EditorGUI.Slider(new Rect(345 + i * 260, Screen.height / 2 - ((20 * weights[i].GetLength(0) * weights[i].GetLength(1)) / 2) + (20 * j * NumOutputLayers) + 20 * f, 150, 20), weights[i][j, f], -1f, 1f);
                    }
                }
            }
        }

        
    }
    void ClearNetwork()
    {
        for(int i = 0; i < NumHiddenLayers; i++)
        {
            for(int j = 0; j < neurons[i].Length; j++)
            {
                neurons[i][j] = 0;
            }
        }
        for(int i = 0; i < outputLayer.Count; i++)
        {
            outputLayer[i] = 0;
        }
    }

    float SoftPlus(float input)
    {
        float output;
        output = Mathf.Log(1 + Mathf.Exp(input));
        return (output);
    }
    float Sigmoid(float input)
    {
        float output;
        output = 1 / (1 + Mathf.Exp(-input));
        return (output);
    }
}

