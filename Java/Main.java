import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Arrays;
import java.util.stream.Collectors;

public class Main {

    public static char[] charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".toCharArray();
    private static String HASHTYPE = "SHA-1";

    public static void main(String[] args) throws NoSuchAlgorithmException {
        String charArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        String password = "ZZZZ";

        MessageDigest digest = MessageDigest.getInstance(HASHTYPE);
        byte[] hashedPassword = digest.digest(password.getBytes());
//        boolean pwFound = false;

        int[] chars = new int[charList.length];
        for (int i = 0; i < chars.length; i++) {
            chars[i] = Character.getNumericValue(charList[i]);
        }
        long start = System.currentTimeMillis();

//        for(int first = 0; first < charList.length && !pwFound; first++){
//            for(int second = 0; second < charList.length && !pwFound; second++){
//                for(int third = 0; third < charList.length && !pwFound; third++){
//                    for(int fourth = 0; fourth < charList.length && !pwFound; fourth++){
//                        String possiblePW = "" + charList[first] + charList[second] + charList[third] + charList[fourth];
//                        byte[] possibleSolution = digest.digest(possiblePW.getBytes());
//                        if(hashedPassword.equals(possibleSolution)){
//                            pwFound = true;
//                        }
//                    }
//                }
//            }
//        }

        String guessedPW = charArray.chars().parallel().mapToObj(first -> {

            for (char second : charList) {
                for (char third : charList) {
                    for (char fourth : charList) {
                        String possiblePW = "" + (char)first + second + third + fourth;

                        try{
                            MessageDigest digestInsied = MessageDigest.getInstance(HASHTYPE);
                            byte[] possibleSolution = digestInsied.digest(possiblePW.getBytes());
                            if (Arrays.equals(hashedPassword, possibleSolution)) {
                                return possiblePW;
                            }

                        }catch(Exception e){
                            return "EXCEPTION";
                        }
                    }
                }
            }
            return "";
        })
                .filter(item -> item != null && !item.isEmpty())
                .collect(Collectors.joining());

        long end = System.currentTimeMillis();
        System.out.println("total Time: " + (end - start) + "ms and password is " + guessedPW);

    }
}





//WORKING SOLUTION
//String guessedPW = charArray.chars().parallel().mapToObj(first -> {
//
//    for (char second : charList) {
//        for (char third : charList) {
//            for (char fourth : charList) {
//                String possiblePW = "" + (char)first + second + third + fourth;
//                //byte[] possibleSolution = digest.digest(possiblePW.getBytes());
//                if (password.equals(possiblePW)) {
//                    return possiblePW;
//                }
//            }
//        }
//    }
//    return "";
//})
//        .filter(item -> item != null && !item.isEmpty())
//        .collect(Collectors.joining());
