#MaruBatsuGame
isEnd = False       #ゲーム終了フラグ
isTurn = True       #Trueなら○のターン、Falseなら×のターン
inputCoord = [0, 0] #入力座標
winCount = [0, 0]  #勝敗判定カウント

#初期盤面
board = [[" ", "1", "2", "3"], ["1", "-", "-", "-"], ["2", "-", "-", "-"], ["3", "-", "-", "-"]]

while(isEnd == False):
    #盤面表示
    for i in range(len(board)):
        for j in range(4):
            if j < 3:
                print(board[i][j], end = " ")
            else:
                print(board[i][j])

    #縦横の勝敗判定
    for i in range(len(board) - 1):
        if winCount[0] == len(board) - 1 or winCount[1] == len(board) - 1:
            isEnd = True
            break
        else:
            winCount[0] = 0
            winCount[1] = 0
        for j in range(len(board) - 1):
            if board[i + 1][j + 1] == "○":
                winCount[0] += 1
            elif board[i + 1][j + 1] == "×":
                winCount[1] += 1
    for i in range(len(board) - 1):
        if winCount[0] == len(board) - 1 or winCount[1] == len(board) - 1:
            isEnd = True
            break
        else:
            winCount[0] = 0
            winCount[1] = 0
        for j in range(len(board) - 1):
            if board[j + 1][i + 1] == "○":
                winCount[0] += 1
            elif board[j + 1][i + 1] == "×":
                winCount[1] += 1


    #斜めの勝利判定
    winCount[0] = 0
    winCount[1] = 0
    for i in range(len(board) - 1):
        if board[i + 1][i + 1] == "○":
            winCount[0] += 1
        elif board[i + 1][i + 1] == "×":
            winCount[1] += 1
    if winCount[0] == len(board) - 1 or winCount[1] == len(board) - 1:
        isEnd = True
    winCount[0] = 0
    winCount[1] = 0
    for i in range(len(board) - 1):
        if board[i + 1][len(board) - 1 - i] == "○":
            winCount[0] += 1
        elif board[i + 1][len(board) - 1 - i] == "×":
            winCount[1] += 1
    if winCount[0] == len(board) - 1 or winCount[1] == len(board) - 1:
        isEnd = True


    #どちらかがいずれかの勝利条件を満たしていたらループを抜ける
    if isEnd == True:
        break

    #座標入力
    if isTurn == True:
        coord = input("○の番です、縦-横の順で座標を入力してください -> ")
        for i in range(2):
            inputCoord[i] = int(coord[i])
        #マスの置き換え
        board[inputCoord[0]][inputCoord[1]] = "○"
        #ターンの変更
        isTurn = False
    else:
        coord = input("×の番です、縦-横の順で座標を入力してください -> ")
        for i in range(2):
            inputCoord[i] = int(coord[i])
        #マスの置き換え
        board[inputCoord[0]][inputCoord[1]] = "×"
        #ターンの変更
        isTurn = True

#勝者を表示して終了する
if isTurn == True:
    print("×の勝利です！")
else:
    print("○の勝利です！")
