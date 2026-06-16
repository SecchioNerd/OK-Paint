using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using System.Collections.Generic;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;

namespace okpaint;

public partial class MainWindow : Window
{
    //selection of the tools for draw
    private enum strumento{
        LINEA,
        RETTANGOLO,
        ELLISSE,
        TESTO,
        GOMMA,
        NULLA
    }
    private strumento selezioneStrumento = strumento.NULLA;

    //liste dei componeti che vengono disegnati
    public Line linea = new Line();
    public Rectangle rettangolo = new Rectangle();
    public Ellipse ellisse = new Ellipse();
    public TextBlock testo = new TextBlock();

    public TextBox scritturaTesto = new TextBox{
        //Name="InputBox",
        Width=50
    };

    public int ofsetTextBox = 0;
    public bool isDrawing;
    public bool isWriting;
    public bool isVisualizzatoTextBox = false;
    public bool isShiftPress;
    public bool isCtrlPress;

    public IBrush lineColor = Brushes.White;
    public int lineSpessore = 5;

    public Point pointStartRettangle;
    public Point pointStartEllisse;

    public MainWindow()
    {
        isDrawing = false;
        isWriting = false;
        isCtrlPress = false;
        isShiftPress = false;

        InitializeComponent();

        //Controllo Tasti Tastiera
        DrawingCanvas.KeyDown += SelezionaStrumentoKeyb;

        DrawingCanvas.PointerPressed += Canvas_PointerPressed;
        DrawingCanvas.PointerMoved += Canvas_PointerMoved;
        DrawingCanvas.PointerReleased += Canvas_PointerReleased;
    }

//    M E T O D I    D I    I N D I R I Z Z A M E N T O
    public void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e){
        switch (selezioneStrumento){
            case (strumento.LINEA):
                InizioLinea(sender,e);
                break;
            case (strumento.RETTANGOLO):
                InizioRettangolo(sender,e);
                break;
            case (strumento.ELLISSE):
                InizioEllisse(sender,e);
                break;
            case (strumento.TESTO):
                InizioTesto(sender,e);
                break;
            case(strumento.GOMMA):
                InizioGomma(sender,e);
                break;
        }
        return;
    }

    public void Canvas_PointerMoved(object? sender, PointerEventArgs e){
        switch (selezioneStrumento){
            case (strumento.LINEA):
                AggiornaLinea(sender,e);
                break;
            case (strumento.RETTANGOLO):
                AggiornaRettangolo(sender,e);
                break;
            case (strumento.ELLISSE):
                AggiornaEllisse(sender,e);
                break;
            case (strumento.GOMMA):
                AggiornaGomma(sender,e);
                break;
            
        }
        return;
    }

    public void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e){
        switch (selezioneStrumento){
            case (strumento.LINEA):
                FineLinea(sender,e);
                break;
            case (strumento.RETTANGOLO):
                FineRettangolo(sender,e);
                break;
            case (strumento.ELLISSE):
                FineEllisse(sender,e);
                break;
            case (strumento.TESTO):
                FineTesto(sender,e);
                break;
            case (strumento.GOMMA):
                FineGomma(sender,e);
                break;
        }
        return;
    }

// Selezione Strumento con Lista schermo:
    public void SelezionaStrumentoGUI(object? sender, RoutedEventArgs e){
        if (sender is not RadioButton rb)
            return;
        if (rb.IsChecked != true) 
            return;

        switch (rb.Content?.ToString()){
            case "Linea":
                selezioneStrumento = strumento.LINEA;
                break;
            case "Rettangolo":
                selezioneStrumento = strumento.RETTANGOLO;
                break;
            case "Ellisse":
                selezioneStrumento = strumento.ELLISSE;
                break;
            case "Testo":
                selezioneStrumento = strumento.TESTO;
                break;
            case "Gomma":
                selezioneStrumento = strumento.GOMMA;
                break;
        }
    }

    public void SelezionaStrumentoKeyb(object? sender,KeyEventArgs e){
        
        
        switch (e.Key){
            case (Key.L):
                selezioneStrumento = strumento.LINEA;
                break;
            case (Key.R):
                selezioneStrumento = strumento.RETTANGOLO;
                break;
            case (Key.E):
                selezioneStrumento = strumento.ELLISSE;
                break;
            case (Key.T):
                selezioneStrumento = strumento.TESTO;
                break;
            case (Key.G):
                selezioneStrumento = strumento.GOMMA;
                break;           
        }

        return;
    }

//    L I N E A
    public void InizioLinea (object? sender, PointerPressedEventArgs e){
        if(selezioneStrumento != strumento.LINEA)
            return;

        isDrawing = true;

        linea = new Line{
            StartPoint = e.GetPosition(DrawingCanvas),
            EndPoint = e.GetPosition(DrawingCanvas),
            Stroke = lineColor,
            StrokeThickness = lineSpessore
        };

        DrawingCanvas.Children.Add(linea);
    }

    public void AggiornaLinea(object? sender, PointerEventArgs e){
        if(!(isDrawing && linea!=null)){
            return;
        }
        
        linea.EndPoint = e.GetPosition(DrawingCanvas);
            
        DrawingCanvas.KeyDown += Shift;
    }

    public void FineLinea(object? sender, PointerReleasedEventArgs e){
        if (!isDrawing || linea == null)
            return;

        linea.EndPoint = e.GetPosition(DrawingCanvas);
        DrawingCanvas.KeyDown += Shift;
        //selezioneStrumento = strumento.NULLA;
        isDrawing = false;
    }

//     R E T T A N G O L O
    public void InizioRettangolo (object? sender, PointerPressedEventArgs e){
        if(selezioneStrumento != strumento.RETTANGOLO)
            return;

        isDrawing = true;

        pointStartRettangle = e.GetPosition(DrawingCanvas);

        DrawingCanvas.Children.Add(rettangolo);
    }

    public void AggiornaRettangolo(object? sender, PointerEventArgs e){
        if(!(isDrawing)){
            return;
        }

        Line lunghezzaRettangle= new Line{
            StartPoint = pointStartRettangle,
            EndPoint = new Point(e.GetPosition(DrawingCanvas).X,pointStartRettangle.Y)
        };
        Line altezzaRettangle= new Line{
            StartPoint = pointStartRettangle,
            EndPoint = new Point(pointStartRettangle.X,e.GetPosition(DrawingCanvas).Y)
        };
        
        //calcolo centro base e altezza   ->  CENTRO TEGLIERE
        double distanzaTopLeftX = lunghezzaRettangle.StartPoint.X;
        double distanzaTopLeftY = altezzaRettangle.StartPoint.Y;
        double lunghezza = lunghezzaRettangle.EndPoint.X-lunghezzaRettangle.StartPoint.X;
        double altezza = altezzaRettangle.EndPoint.Y-altezzaRettangle.StartPoint.Y;

        if(distanzaTopLeftX>lunghezzaRettangle.EndPoint.X)
            distanzaTopLeftX = lunghezzaRettangle.EndPoint.X;
        if(distanzaTopLeftY > altezzaRettangle.EndPoint.Y)
            distanzaTopLeftY = altezzaRettangle.EndPoint.Y;

        if(lunghezza<0)
            lunghezza*=-1;
        if(altezza<0)
            altezza*=-1;

        rettangolo.Width = lunghezza;
        rettangolo.Height = altezza;
        rettangolo.Stroke = lineColor;
        rettangolo.StrokeThickness = lineSpessore;

        Canvas.SetLeft(rettangolo,distanzaTopLeftX);
        Canvas.SetTop(rettangolo,distanzaTopLeftY);
        
        DrawingCanvas.KeyDown += EscUscitaComando;
    }

    public void FineRettangolo(object? sender, PointerReleasedEventArgs e){
        if (!isDrawing)
            return;

        //selezioneStrumento = strumento.NULLA;
        isDrawing = false;
        rettangolo = new Rectangle();
    }

//     E L L I S S E
    public void InizioEllisse (object? sender, PointerPressedEventArgs e){
        if(selezioneStrumento != strumento.ELLISSE)
            return;

        isDrawing = true;

        pointStartEllisse = e.GetPosition(DrawingCanvas);

        DrawingCanvas.Children.Add(ellisse);
    }

    public void AggiornaEllisse(object? sender, PointerEventArgs e){
        if(!(isDrawing)){
            return;
        }

        Line lunghezzaEllisse= new Line{
            StartPoint = pointStartEllisse,
            EndPoint = new Point(e.GetPosition(DrawingCanvas).X,pointStartRettangle.Y)
        };
        Line altezzaEllisse= new Line{
            StartPoint = pointStartEllisse,
            EndPoint = new Point(pointStartRettangle.X,e.GetPosition(DrawingCanvas).Y)
        };
        
        //calcolo centro base e altezza
        double distanzaTopLeftX = lunghezzaEllisse.StartPoint.X;
        double distanzaTopLeftY = altezzaEllisse.StartPoint.Y;
        double lunghezza = lunghezzaEllisse.EndPoint.X-lunghezzaEllisse.StartPoint.X;
        double altezza = altezzaEllisse.EndPoint.Y-altezzaEllisse.StartPoint.Y;

        if(distanzaTopLeftX>lunghezzaEllisse.EndPoint.X)
            distanzaTopLeftX = lunghezzaEllisse.EndPoint.X;
        if(distanzaTopLeftY > altezzaEllisse.EndPoint.Y)
            distanzaTopLeftY = altezzaEllisse.EndPoint.Y;

        if(lunghezza<0)
            lunghezza*=-1;
        if(altezza<0)
            altezza*=-1;

        ellisse.Width = lunghezza;
        ellisse.Height = altezza;
        ellisse.Stroke = lineColor;
        ellisse.StrokeThickness = lineSpessore;

        Canvas.SetLeft(ellisse,distanzaTopLeftX);
        Canvas.SetTop(ellisse,distanzaTopLeftY);
    }

    public void FineEllisse(object? sender, PointerReleasedEventArgs e){
        if (!isDrawing)
            return;

        //selezioneStrumento = strumento.NULLA;
        isDrawing = false;
        ellisse = new Ellipse();
    }

//    T E S T O
    public void InizioTesto (object? sender, PointerPressedEventArgs e){
        if(selezioneStrumento != strumento.TESTO)
            return;

        isDrawing = true;
        isWriting = true;

        var pointStart = e.GetPosition(DrawingCanvas);
        
        testo = new TextBlock{
            Text = "",
            FontSize = 20,
            Foreground = lineColor
        };

        Canvas.SetLeft(testo,pointStart.X);
        Canvas.SetTop(testo,pointStart.Y);

        DrawingCanvas.Children.Add(testo);
    }

    public void FineTesto(object? sender, PointerReleasedEventArgs e){
        if (!isDrawing)
            return;

        if(!isVisualizzatoTextBox){
            DrawingCanvas.Children.Add(scritturaTesto);
            isVisualizzatoTextBox = true;
        }

        scritturaTesto.KeyDown += ControlloInvio;
        scritturaTesto.TextChanged += aggiornaText;
    }

    public void ControlloInvio(object? sender, KeyEventArgs e){
        if(e.Key == Key.Enter){
            testo = new TextBlock();
            scritturaTesto.Text = "";
            DrawingCanvas.Children.Remove(scritturaTesto);
            isVisualizzatoTextBox = false;
            isWriting = false;
            isDrawing = false;
        }
    }

    public void aggiornaText(object? sender, TextChangedEventArgs e){
        testo.Text = scritturaTesto.Text;
    }

//    G O M M A
    public void InizioGomma(object? sender, PointerPressedEventArgs e){
        if(selezioneStrumento != strumento.GOMMA)
            return;

        isDrawing = true;
        int dimEllisse = 40;

        Ellipse puntoCancellato = new Ellipse{
            Height = dimEllisse,
            Width = dimEllisse,
            Fill = DrawingCanvas.Background
        };
        
        Canvas.SetLeft(puntoCancellato,e.GetPosition(DrawingCanvas).X-dimEllisse/2);
        Canvas.SetTop(puntoCancellato,e.GetPosition(DrawingCanvas).Y-dimEllisse/2);

        DrawingCanvas.Children.Add(puntoCancellato);
    }

    public void AggiornaGomma(object? sender, PointerEventArgs e){
        if(!isDrawing)
            return;

        int dimEllisse = 40;

        Ellipse puntoCancellato = new Ellipse{
            Height = dimEllisse,
            Width = dimEllisse,
            Fill = DrawingCanvas.Background
        };
        
        Canvas.SetLeft(puntoCancellato,e.GetPosition(DrawingCanvas).X-dimEllisse/2);
        Canvas.SetTop(puntoCancellato,e.GetPosition(DrawingCanvas).Y-dimEllisse/2);

        DrawingCanvas.Children.Add(puntoCancellato);
    }

    public void FineGomma(object? sender, PointerReleasedEventArgs e){
        if(selezioneStrumento != strumento.GOMMA && !isDrawing)
            return;
        isDrawing = false;
    }
//   P U L I S C I     T U T T O
public void PulisciTutto(object? sender, RoutedEventArgs e){
    DrawingCanvas.Children.Clear();
}

//    G E S T I O N E    T A S T I
    public void Shift(object? sender, KeyEventArgs e){
        if(e.Key != Key.LeftShift)
            return;

        double lung = linea.StartPoint.X-linea.EndPoint.X;
        double alt = linea.StartPoint.Y-linea.EndPoint.Y;

        if(lung<0)
            lung*=-1;
        if(alt<0)
            alt*=-1;
            
        if(lung<=alt)
            linea.EndPoint = new Point(linea.StartPoint.X,linea.EndPoint.Y);
        else
            linea.EndPoint = new Point(linea.EndPoint.X,linea.StartPoint.Y);
        return;
    }

    public void EscUscitaComando(object? sender, KeyEventArgs e){
        if(e.Key != Key.Escape)
            return;
        
        switch(selezioneStrumento){
            case (strumento.LINEA):
                DrawingCanvas.Children.Remove(linea);
                break;
            case (strumento.RETTANGOLO):
                DrawingCanvas.Children.Remove(rettangolo);
                break;
            case (strumento.ELLISSE):
                DrawingCanvas.Children.Remove(ellisse);
                break;
            case (strumento.TESTO):
                DrawingCanvas.Children.Remove(testo);
                break;
            
        }
    }
}