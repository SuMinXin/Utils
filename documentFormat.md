# Introduction 
Document Format for [Nani Resource Platform]

# HttpMethod
### **<font color=#66B3FF>[GET] </font>**
> description

### **<font color=#1AFD9C>[POST] </font>**
> description

### **<font color=#FFA042>[PUT] </font>**
> description

### **<font color=#FF0000>[DELETE] </font>**
> description
---

# Tables
### [General]
  > 欄位說明
  >| Name       | Meaning
  > ----------  | ---------------------
  > dataName    | Description
  
### [Remark]
  > 欄位說明
  > | Name      |  Meaning   | Remark  |
  > | --------- | ---------- |---------|
  > | dataName  | Description| Remark  |


### [Required]
  > <b><font color=#FF0000>* :Required</font></b><br/>
  > |  | Name        | Meaning |
  > | ---------| ---------   |---------|
  > | <b><font color=#FF0000>*</font></b> | dataName  | Description
  > | <b><font color=#FF0000>*</font></b> | dataName       | Description
  > | | dataName   | Description
  
### [System Code]
| Error Code | Meaning               |
| ---------- | --------------------- |
| 0001       | Start                 |
| 0200       | Succeed               |
|<font color=#84C1FF>LOGIN</font>    |
| 0100       | First Time            |
| 0101       | OneClub Token Error   |
| 0102       | User must be Editor   |
| 0103       | Token Expired         |
|<font color=#84C1FF>EDIT</font>     |
| 0201       | Insert Failed         |
| 0211       | Update Failed         |
|<font color=#84C1FF>Format</font>   |
| 0400       | Data Null             |
| 0401       | Data Already Exist    |
|<font color=#84C1FF>System Error</font>|
| 9999       | Exception             |

---

# Document Required
* ### Header
  > 
  ```json
  {
    "Authorization": "<token>"
  }
  ```
* ### Payload
  *none*

* ### Response
  ```json
  ```
