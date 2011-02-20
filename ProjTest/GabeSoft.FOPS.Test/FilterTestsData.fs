module FilterTestsData

// characters not allowed in file or directory names
let reserved = [ '<'; '>'; ':'; '"'; '/'; '\\'; '|'; '?'; '*' ]

let patterns1 = [
  // all files in 'C:\'
  @"C:\*", [   
    @"C:\a", true
    @"C:\a.txt", true
    @"C:\a\b", false 
    @"C:\a\b\c.txt", false ]
  // all files in 'C:\' that contain the string 'foo'
  @"C:\*foo*", [   
    @"C:\foo", true
    @"C:\foo.pdf", true
    @"C:\afoob.pdf", true
    @"C:\bfoo.txt", true
    @"C:\foo2", true
    @"C:\a.txt", false
    @"C:\foo\foo.txt", false
    @"C:\b\foo.doc", false 
    @"C:\a\b\c\foo\foo.txt", false
    @"C:\a\b\foo.doc", false ]
  // all files in all subdirectories of 'C:\' at any level
  @"C:\*\*", [
    @"C:\a", true
    @"C:\a.txt", true
    @"C:\a\b", true
    @"C:\a\b.txt", true                       
    @"C:\a\b\c\f.doc", true ]
  // all files in all subdirectories of 'C:\' at any level that are in a folder named 'a'
  @"C:\*\a\*", [
    @"C:\f.pdf", false
    @"C:\a\f.pdf", true
    @"C:\b\c\d\a\f.pdf", true
    @"C:\b\c\d\a\e\f\g\f.pdf", false ]
  // all .pdf files in all folders prefixed with 'b' that are found in 'C:\a\'
  @"C:\a\b*\*.pdf", [
    @"C:\b1\f.pdf", false
    @"C:\a\b\f.pdf", true
    @"C:\a\b2\f2.pdf", true
    @"C:\a\b2\f2.doc", false
    @"C:\c\a\b\f.doc", false ]
  // all .txt files in all folders named b that are found in all subdirectories of 'C:\a\' one level deep      
  @"C:\a\?*\c\*.txt", [
    @"C:\b\c\f.txt", false
    @"C:\a\b\c\f.txt", true
    @"C:\a\b\c\txt", false
    @"C:\a\b\c\f.pdf", false
    @"C:\a\c\f.txt", false
    @"C:\a\b\d\c\f.txt", false ]
  // same as C:\a\*\b\*
  @"C:\a\*\*\*\e\*.*", [
    @"C:\a\e", false
    @"C:\a\e\f1", true
    @"C:\a\e\f1.txt", true
    @"C:\a\b\e\f1", true
    @"C:\a\b\c\e\f1", true
    @"C:\a\b\c\d\e\f1", true
    @"C:\a\b\c\e\f\f2", false ]
  @"C:\a\b\?c\*\f*.pd?", [
    @"C:\a\b\1c\f1.pdf", true
    @"C:\a\e\b\1c\f1.pdf", false
    @"C:\e\a\b\1c\f1.pdf", false
    @"C:\a\b\c\f1.pdf", false
    @"C:\a\b\1c\a1.pdf", false
    @"C:\a\b\2c\f2.pdc", true
    @"C:\a\b\1c\d\f1.pdf", true
    @"C:\a\b\1c\d\e\f1.pdf", true ]
  @"C:\?*\b\c\*\e?\file?.*", [
    @"C:\b\c\e1\file1.pdf", false
    @"C:\a\b\c\e1\file1.pdf", true            
    @"C:\a\b\c\d\e1\file1.pdf", true            
    @"C:\a\b\c\d\e\e1\file1.pdf", true            
    @"C:\a\a2\b\c\d\e\e1\file1.pdf", false
    @"C:\a\b1\c\d\e\e1\file1.pdf", false
    @"C:\a\b\c\d\e\file1.pdf", false
    @"C:\a\b\c\d\e1\file", false ]
  @"C:\a\b\c?\d\*", [
    @"C:\a\b\c\d\f.txt", false
    @"C:\a\b\c1\d\f.txt", true
    @"C:\b\c1\d\f.txt", false
    @"C:\a\b1\c1\d\f.txt", false ]
  @"C:\a\?*\c\fi?e.doc", [
    @"C:\a\b\c\file.doc", true
    @"C:\a\b\c\fire.doc", true
    @"C:\a\b\c\pipe.doc", false
    @"C:\a\b\b1\c\file.doc", false
    @"C:\a\b\c\file1.doc", false ]
  @"C:\a\b\c\f.txt", [
    @"C:\a\b\c\f.txt", true
    @"C:\a\b\c\f2.txt", false
    @"C:\a\b1\c\f.txt", false
    @"C:\a\b\f.txt", false ]
  @"C:\a\*\bin\*\*.dll", [
    @"C:\a\bin\f1.dll", true
    @"C:\a\b\bin\f2.dll", true
    @"C:\a\b\bin\c\f3.dll", true
    @"C:\a\b\c\bin\d\e\f3.dll", true
    @"C:\a\b\bin\c\bin\bin\d\f4.dll", true ]        
  @"C:\*a*\b\test.doc", [
    @"C:\a\b\test.doc", true
    @"C:\da\b\test.doc", true
    @"C:\da2\b\test.doc", true
    @"C:\b\test.doc", false
    @"C:\a\b\t.doc", false
    @"C:\a\b1\test.doc", false ]]

let patterns2 = [
   // pattern
   @"C:\a\b\*\f\?*\h\*", 
   // excludes
   [  @"C:\a\b\*\f\g\h\*"
      @"C:\a\b\*\f\?*\h\*.tmp"
      @"C:\a\b\*\f\?*\h\*.pdb" ], 
   // test data
   [  @"C:\a\b\f\g\h\f1.txt", false
      @"C:\a\b\f\g\h\f2.txt", false
      @"C:\a\b\f\g1\h\f3.txt", true
      @"C:\a\b\f\g2\h\f3.doc", true
      @"C:\a\b\f\g3\h\f4.tmp", false
      @"C:\a\b\f\g4\h\f5.pdb", false
      @"C:\a\b\c\f\g1\h\f3.txt", true
      @"C:\a\b\c\f\g2\h\f3.doc", true
      @"C:\a\b\c\f\g1\h\f3.tmp", false
      @"C:\a\b\c\f\g2\h\f3.pdb", false
      @"C:\a\b\c\f\g3\h\f4.tmp", false
      @"C:\a\b\c\f\g4\h\f5.pdf", true
      @"C:\a\b\c\d\f\g1\h\f3.txt", true
      @"C:\a\b\c\d\f\g2\h\f3.doc", true
      @"C:\a\b\c\d\f\g1\h\f3.tmp", false
      @"C:\a\b\c\d\f\g2\h\f3.pdf", true
      @"C:\a\b\c\d\f\g3\h\f4.tmp", false
      @"C:\a\b\c\d\f\g4\h2\f5.pdf", false ]]
