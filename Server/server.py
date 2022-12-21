import socket
import threading
import json

SIZE = 1024
FORMAT = "utf-8"
DISCONNECT_MSG = "!DISCONNECT"
IS_HOST_MSG = "!IS_HOST"
isRoomAvaliable = False
rooms = []

class Room:
    def __init__(self, host, hostIP, enemy, enemyIP, hostMsg, enemyMsg):
        self.host = host
        self.hostIP = hostIP
        self.enemy = enemy
        self.enemyIP = enemyIP
        self.hostMsg = hostMsg
        self.enemyMsg = enemyMsg



def handle_msg(msg, room, ip):
    msg = json.loads(msg)
    if room.hostIP == ip:
        msg['EnemyPosX'] = room.enemyMsg['HostPosX'] if room.enemyMsg != 0 else 0
        room.hostMsg = msg
        
        return json.dumps(room.hostMsg)
        
    if room.enemyIP == ip:
        msg['BallPosX'] = room.hostMsg['BallPosX'] if room.hostMsg != 0 else 0
        msg['BallPosY'] = room.hostMsg['BallPosY']
        msg['EnemyPosX'] = room.hostMsg['HostPosX']
        room.enemyMsg = msg

        return json.dumps(room.enemyMsg)
   

def handle_client(conn, addr):
    print(f"[NEW CONNECTION] {addr} connected")
    thisRoom = None

    if len(rooms):
        for room in rooms:
            if room.enemy == 0:
                room.enemy = conn
                room.enemyIP = addr
                thisRoom = room
    else:
        r = Room(0, 0, 0, 0, 0, 0)
        r.host = conn
        r.hostIP = addr
        rooms.append(r)
        thisRoom = r
    
    print(thisRoom.__dict__)

    connected = True
    while connected:
        if rooms.__contains__(thisRoom) == False:
            break
        try:
            msg = conn.recv(SIZE).decode(FORMAT)
        except:
            pass

        #print(f"[{addr}] {msg}")
        
        if msg == DISCONNECT_MSG:
            connected = False

        elif msg == IS_HOST_MSG:

            if thisRoom.enemyIP == addr:
                msg = "-1"
            else:
                msg = "1"
     
        elif thisRoom != None:
            msg = handle_msg(msg, thisRoom, addr)

        conn.send(msg.encode(FORMAT))
        
    conn.close()
    try:
        rooms.remove(thisRoom)
    except Exception as e:
        pass
    print(f"[CLOSED CONNECTION] Closed connection with {addr}")


def server():
    print("[STARTING] Server is starting")
    # get the hostname
    host = socket.gethostbyname(socket.gethostname())
    port = 5000  # initiate port no above 1024

    server = socket.socket()  # get instance
    server.bind((host, port))  # bind host address and port together
    server.listen()
    print(f"[LISTENING] Server is listening on {host}:{port}")

    while True:
        conn, addr = server.accept()  # accept new connection
        thread = threading.Thread(target=handle_client, args=(conn,addr))
        thread.start()
        print(f"[ACTIVE CONNECTIONS] {threading.active_count() - 1}")
        


if __name__ == '__main__':
    server()