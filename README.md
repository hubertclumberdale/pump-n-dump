# Descrizione Generale del Gioco

Il gioco mette il giocatore al centro di una sfida strategica per portare la propria azienda a 100 punti di valore, interagendo con una fila di persone che rappresentano diverse aziende in competizione. Ogni turno, il giocatore utilizza carte per influenzare il valore delle aziende e deve anche evitare un pericoloso personaggio speciale, lo Sbirro, che può apparire in fondo alla fila.

## Obiettivo del Gioco

Portare il valore dell'azienda assegnata al giocatore a 100 punti per vincere la partita. Evitare condizioni di sconfitta, che includono lo Sbirro e i valori delle aziende.

### Condizioni di Vittoria

- L'azienda del giocatore raggiunge 100 punti.

### Condizioni di Sconfitta

- L'azienda assegnata al giocatore scende a 0 punti.
- Un'altra azienda (non del giocatore) raggiunge 100 punti.
- Lo Sbirro arriva alla posizione del giocatore nella fila.
- Un’azienda (diversa da quella del giocatore) scende a 0 punti, causando la sua rimozione dal gioco. Se ciò rende impossibile raggiungere la vittoria, il giocatore perde.

## Setup del Gioco

- **Aziende**: Ci sono 4 aziende (A, B, C, D), ognuna con un valore iniziale di 20 punti.
- **Assegnazione al giocatore**: All'inizio del gioco, al giocatore viene assegnata casualmente una delle 4 aziende. Questa sarà l'azienda che il giocatore deve portare a 100 punti.
- **Fila iniziale**: Una fila di 9 persone viene generata casualmente, ognuna appartenente a una delle 4 aziende.
    - Nota: Lo Sbirro non può essere presente fra le prime 9 persone della fila iniziale.
- **Carte iniziali**: Il giocatore inizia con 3 carte casuali pescate dal mazzo.

## Lo Sbirro

Lo Sbirro è un personaggio speciale che rappresenta una minaccia diretta al giocatore.

- **Probabilità di apparizione**: Dopo il setup iniziale, ad ogni turno c'è una probabilità del 5% che lo Sbirro venga aggiunto in fondo alla fila.
- **Comportamento**: Lo Sbirro avanza nella fila come un normale cliente. Se lo Sbirro raggiunge la posizione del giocatore, la partita termina immediatamente con una sconfitta.
- **Gestione dello Sbirro**:
    - Il giocatore può manipolare la fila o usare carte per spostare o rimuovere lo Sbirro.
    - Carte come Freq o Shuffle possono randomizzare la fila, spostando lo Sbirro lontano dal giocatore.

## Turni e Fila

### Struttura di un Turno

1. **Aggiornamento della Fila**:
     - La persona davanti nella fila interagisce con il giocatore (e viene rimossa).
     - Una nuova persona casuale (A, B, C o D) viene aggiunta in fondo alla fila, con una probabilità del 5% che sia lo Sbirro.
     - Se un’azienda è stata eliminata (perché ha raggiunto 0 punti), le persone associate a quell’azienda non appariranno più nella fila.

2. **Pesca di una Carta**:
     - Il giocatore riceve una carta casuale dal mazzo, avendo sempre 3 carte in mano all'inizio di ogni turno.

3. **Giocare una Carta**:
     - Il giocatore sceglie una delle 3 carte in mano e applica il suo effetto sulla persona che ha davanti nella fila. L’effetto influenzerà il valore dell’azienda associata a quella persona (o più aziende, a seconda della carta).

### Fine della Partita

La partita termina immediatamente quando si verifica una delle seguenti condizioni:

- **Vittoria**: L'azienda del giocatore raggiunge 100 punti.
- **Sconfitta**:
    - L'azienda del giocatore scende a 0 punti.
    - Un’altra azienda (non del giocatore) raggiunge 100 punti.
    - Lo Sbirro raggiunge la posizione del giocatore nella fila.
    - Un’azienda (diversa da quella del giocatore) scende a 0 punti, causando l’eliminazione di quell’azienda. Se ciò rende impossibile vincere, il giocatore perde.