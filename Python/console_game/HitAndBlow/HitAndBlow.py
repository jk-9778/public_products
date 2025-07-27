#Hit&Blow
import random

pNum = []
cNum = []
inputCheck = False
isClear = False
inputNum = ""
inputCount = 1

#CP用の4桁の数字を生成
set = [1, 2, 3, 4, 5, 6, 7, 8, 9]
cNum = random.sample(set, 4)
#デバッグ用に答えを表示
#print(cNum)

while(isClear == False):

    #プレイヤー入力
    while(inputCheck == False):
        #4桁の数字を入力させる
        inputNum = input("第" + str(inputCount) + "回目 プレイヤーの入力 -> ")

        #ちゃんと4桁の数字であるかをチェック
        if len(inputNum) != 4:
            print("入力エラー！ 0～9の4桁の数字を入力してください\n")
        else:
            for i in range(4):
                if inputNum[i].isdigit():
                    if i == 3:
                        pNum.clear()
                        for i in range(len(inputNum)):
                            pNum.append(int(inputNum[i]))
                        inputCheck = True
                    else:
                        pass
                else:
                    print("入力エラー！ 0～9の4桁の数字を入力してください\n")
                    break
    
    #入力状態を初期化
    inputCheck = False
    inputCount += 1
    #Hit、Blow判定
    hitCount = 0
    blowCount = 0
    for i in range(4):
        if pNum[i] == cNum[i]:
            hitCount += 1
        else:
            for j in range(4):
                if pNum[i] == cNum[j]:
                    blowCount += 1
                else:
                    pass

    print(str(hitCount) + "Hit " + str(blowCount) + "Blow\n")
    if hitCount == 4:
        isClear = True

print(inputNum + "で正解です！ おめでとうございます！")



